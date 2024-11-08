﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using safetool.Data;
using safetool.Models;

namespace safetool.Controllers
{
    [Authorize(Roles = "Administrador, Operador")]
    public class FormSubmissionsController : Controller
    {
        private readonly SafetoolContext _context;

        public FormSubmissionsController(SafetoolContext context)
        {
            _context = context;
        }

        // GET: FormSubmissions
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NumberSortParm"] = string.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewData["LocationSortParm"] = sortOrder == "Location" ? "location_desc" : "Location";
            ViewData["AreaSortParm"] = sortOrder == "Area" ? "area_desc" : "Area";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var submissions = from s in _context.FormSubmissions
                .Include(f => f.Device)
                .Include(f => f.Device.Area)
                .Include(f => f.Device.Area.Location)
                select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                submissions = submissions.Where(s => s.EmployeeUID.Contains(searchString)
                                          || s.EmployeeName.Contains(searchString)
                                          || s.Device.Area.Name.Contains(searchString)
                                          || s.Device.Area.Location.Name.Contains(searchString)
                                          || s.Device.Model.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "number_desc":
                    submissions = submissions.OrderByDescending(s => s.EmployeeUID);
                    break;
                case "Location":
                    submissions = submissions.OrderBy(s => s.Device.Area.Location.Name);
                    break;
                case "location_desc":
                    submissions = submissions.OrderByDescending(s => s.Device.Area.Location.Name);
                    break;
                case "Area":
                    submissions = submissions.OrderBy(s => s.Device.Area.Name);
                    break;
                case "area_desc":
                    submissions = submissions.OrderByDescending(s => s.Device.Area.Name);
                    break;
                default:
                    submissions = submissions.OrderBy(s => s.EmployeeUID);
                    break;
            }

            int pageSize = 15;
            ViewBag.TotalSubmissions = await submissions.CountAsync();
            return View(await PaginatedList<FormSubmission>.CreateAsync(submissions.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // POST: FormSubmissions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int deviceID, string employeeUID, string employeeName, FormSubmission formSubmission)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var device = await _context.Devices.FindAsync(deviceID);
            if (device == null)
            {
                ModelState.AddModelError("", "El dispositivo no existe.");
                return RedirectToAction("Details", "Devices", new { id = deviceID });
            }

            if (ModelState.IsValid)
            {
                // Buscar si ya existe un registro para este usuario y dispositivo
                var existingSubmission = await _context.FormSubmissions
                    .FirstOrDefaultAsync(f => f.DeviceID == deviceID && f.EmployeeUID == employeeUID);

                if (existingSubmission != null)
                {
                    // Si existe un registro, actualiza la información en lugar de crear uno nuevo
                    existingSubmission.EmployeeName = employeeName;
                    existingSubmission.EmployeeEmail = email;
                    existingSubmission.CreatedAt = DateTime.Now;

                    _context.Update(existingSubmission);  // Actualizamos el registro
                }
                else
                {
                    // Si no existe, creamos un nuevo registro
                    formSubmission.DeviceID = deviceID;
                    formSubmission.EmployeeUID = employeeUID;
                    formSubmission.EmployeeName = employeeName;
                    formSubmission.EmployeeEmail = email;
                    formSubmission.CreatedAt = DateTime.Now;

                    _context.Add(formSubmission);  // Creamos un nuevo registro
                }

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Redirigir de vuelta a la vista de detalles del dispositivo
                return RedirectToAction("Index", "Devices");
            }

            // Si hay un error, regresar a la vista de detalles del dispositivo
            return RedirectToAction("Details", "Devices", new { id = deviceID });
        }

    }
}
