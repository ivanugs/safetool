using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using safetool.Data;
using safetool.Models;

namespace safetool.Controllers
{
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
                submissions = submissions.Where(s => s.EmployeeNumber.Contains(searchString)
                                          || s.EmployeeName.Contains(searchString)
                                          || s.Device.Model.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "number_desc":
                    submissions = submissions.OrderByDescending(s => s.EmployeeNumber);
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
                    submissions = submissions.OrderBy(s => s.EmployeeNumber);
                    break;
            }

            int pageSize = 20;
            return View(await PaginatedList<FormSubmission>.CreateAsync(submissions.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // POST: FormSubmissions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int deviceID, string employeeNumber, string employeeName, FormSubmission formSubmission)
        {
            var device = await _context.Devices.FindAsync(deviceID);
            if (device == null)
            {
                ModelState.AddModelError("", "El dispositivo no existe.");
                return RedirectToAction("Details", "Devices", new { id = deviceID });
            }

            if (ModelState.IsValid)
            {
                // Asignar el DeviceID al formSubmission
                formSubmission.DeviceID = deviceID;
                formSubmission.EmployeeNumber = employeeNumber;
                formSubmission.EmployeeName = employeeName;
                formSubmission.CreatedAt = DateTime.Now;

                // Guardar en la base de datos
                _context.Add(formSubmission);
                await _context.SaveChangesAsync();

                // Redirigir de vuelta a la vista de detalles del dispositivo
                return RedirectToAction("Index", "Devices");
            }

            Console.WriteLine("No es valido");

            // Si hay un error, regresar a la vista de detalles del dispositivo
            return RedirectToAction("Details", "Devices", new { id = deviceID });
        }
    }
}
