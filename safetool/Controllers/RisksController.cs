using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using safetool.Data;
using safetool.Models;

namespace safetool.Controllers
{
    public class RisksController : Controller
    {
        private readonly SafetoolContext _context;

        public RisksController(SafetoolContext context)
        {
            _context = context;
        }

        // GET: Risks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Risks.ToListAsync());
        }

        // GET: Risks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var risk = await _context.Risks
                .FirstOrDefaultAsync(m => m.ID == id);
            if (risk == null)
            {
                return NotFound();
            }

            return View(risk);
        }

        // GET: Risks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Risks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Image,Active,ImageFile")] Risk risk)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el archivo ha sido recibido correctamente
                if (risk.ImageFile == null || risk.ImageFile.Length == 0)
                {
                    ModelState.AddModelError("ImageFile", "Debe subir una imagen.");
                    return View(risk);
                }

                string uniqueFileName = null;

                if (risk.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/risks/");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + risk.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Validar que la carpeta existe
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await risk.ImageFile.CopyToAsync(fileStream);
                    };
                }

                // Guardar los datos del modelo en la base de datos
                var newRisk = new Risk
                {
                    Name = risk.Name,
                    Image = "/images/risks/" + uniqueFileName,
                    Active = risk.Active
                };

                _context.Add(newRisk);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(risk);
        }



        // GET: Risks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var risk = await _context.Risks.FindAsync(id);
            if (risk == null)
            {
                return NotFound();
            }
            return View(risk);
        }

        // POST: Risks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Image,Active,ImageFile")] Risk risk)
        {
            if (id != risk.ID)
            {
                return NotFound();
            }

            var existingRisk = await _context.Risks.FindAsync(id);
            if (existingRisk == null)
            {
                return NotFound();
            }

            // Si no se sube foto nueva, conservar la que ya se tiene almacenada
            if (risk.ImageFile == null)
            {
                risk.Image = existingRisk.Image; // Mantener la imagen existente
            }
            else
            {
                // Obtener la ruta de la imagen anterior que sera reemplazada
                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + existingRisk.Image);
                // Logica para manejar la nueva imagen
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/risks/");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + risk.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Validar si la carpeta de imagenes de risks existe
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); //Si no existe la carpeta, la crea
                }

                // Guardar la nueva imagen
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await risk.ImageFile.CopyToAsync(fileStream);
                }

                // Validar que exista la imagen que se va a reemplezar
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath); // Si existe, elimina la imagen
                }

                // Actualizar la propiedad Image con la nueva ruta
                risk.Image = "/images/risks/" + uniqueFileName;
            }

            // Actualizar los campos
            existingRisk.Name = risk.Name;
            existingRisk.Active = risk.Active;
            existingRisk.Image = risk.Image; // Actualizar la imagen, si se subio una nueva

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar el registro en la base de datos
                    _context.Update(existingRisk);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RiskExists(risk.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(risk);
        }

        private bool RiskExists(int id)
        {
            return _context.Risks.Any(e => e.ID == id);
        }
    }
}
