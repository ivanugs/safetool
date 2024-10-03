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
        public async Task<IActionResult> Index()
        {
            var safetoolContext = _context.FormSubmissions.Include(f => f.Device);
            return View(await safetoolContext.ToListAsync());
        }

        // GET: FormSubmissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formSubmission = await _context.FormSubmissions
                .Include(f => f.Device)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (formSubmission == null)
            {
                return NotFound();
            }

            return View(formSubmission);
        }

        // POST: FormSubmissions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ID, [Bind("EmployeeNumber,EmployeeName,DeviceID,CreatedAt")] FormSubmission formSubmission)
        {
            var device = await _context.Devices.FindAsync(ID);
            if (device == null)
            {
                ModelState.AddModelError("", "El dispositivo no existe.");
                return RedirectToAction("Details", "Devices", new { id = ID });
            }

            if (ModelState.IsValid)
            {
                // Asignar el DeviceID al formSubmission
                formSubmission.DeviceID = ID;
                formSubmission.CreatedAt = DateTime.UtcNow;

                // Guardar en la base de datos
                _context.Add(formSubmission);
                await _context.SaveChangesAsync();

                // Redirigir de vuelta a la vista de detalles del dispositivo
                return RedirectToAction("Details", "Devices", new { id = ID });
            }

            // Si hay un error, regresar a la vista de detalles del dispositivo
            return RedirectToAction("Details", "Devices", new { id = ID });
        }



    }
}
