using System;
using System.Collections.Generic;
using System.Linq;
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
    public class PPEsController : Controller
    {
        private readonly SafetoolContext _context;

        public PPEsController(SafetoolContext context)
        {
            _context = context;
        }

        // GET: PPEs
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var ppes = from s in _context.PPEs select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                ppes = ppes.Where(s => s.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    ppes = ppes.OrderByDescending(s => s.Name);
                    break;
                default:
                    ppes = ppes.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 20;
            return View(await PaginatedList<PPE>.CreateAsync(ppes.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: PPEs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPE = await _context.PPEs
                .FirstOrDefaultAsync(m => m.ID == id);
            if (pPE == null)
            {
                return NotFound();
            }

            return View(pPE);
        }

        // GET: PPEs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PPEs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Image,Active,ImageFile")] PPE pPE)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el archivo ha sido recibido correctamente
                if (pPE.ImageFile == null || pPE.ImageFile.Length == 0)
                {
                    ModelState.AddModelError("ImageFile", "Debe subir una imagen.");
                    return View(pPE);
                }

                string uniqueFileName = null;

                if (pPE.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ppe/");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + pPE.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    // Validar que la carpeta existe
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await pPE.ImageFile.CopyToAsync(fileStream);
                    }
                }

                // Guardar los datos del modelo en la base de datos
                var ppe = new PPE
                {
                    Name = pPE.Name,
                    Image = "/images/ppe/" + uniqueFileName,
                    Active = pPE.Active
                };

                _context.Add(ppe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pPE);
        }



        // GET: PPEs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPE = await _context.PPEs.FindAsync(id);
            if (pPE == null)
            {
                return NotFound();
            }
            return View(pPE);
        }

        // POST: PPEs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Image,Active,ImageFile")] PPE pPE)
        {
            if (id != pPE.ID)
            {
                return NotFound();
            }
            
            var existingPPE = await _context.PPEs.FindAsync(id);
            if (existingPPE == null)
            {
                return NotFound();
            }

            // Si no se sube foto nueva, conservar la que ya se tiene almacenada
            if (pPE.ImageFile == null)
            {
                pPE.Image = existingPPE.Image; // Mantener la imagen existente
            }
            else
            {
                // Obtener la ruta de la imagen anterior que sera reemplazada
                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + existingPPE.Image);
                // Logica para manejar la nueva imagen
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ppe/");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + pPE.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Validar si la carpeta de imagenes de PPE existe
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // Si no existe la carpeta, la crea
                }

                // Guardar la nueva imagen
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await pPE.ImageFile.CopyToAsync(fileStream);
                }

                
                // Validar que exista la imagen que se reemplazara
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath); // Si existe, elimina la imagen
                }

                // Actualizar la propiedad Image con la nueva ruta
                pPE.Image = "/images/ppe/" + uniqueFileName;
            }

            // Actualizar los campos
            existingPPE.Name = pPE.Name;
            existingPPE.Active = pPE.Active;
            existingPPE.Image = pPE.Image; // Actualizar la imagen si se subió una nueva

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar el registro en la base de datos
                    _context.Update(existingPPE);
                    await _context.SaveChangesAsync();  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PPEExists(pPE.ID))
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
            return View(pPE);
        }

        private bool PPEExists(int id)
        {
            return _context.PPEs.Any(e => e.ID == id);
        }

    }
}
