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
    public class AreasController : Controller
    {
        private readonly SafetoolContext _context;

        public AreasController(SafetoolContext context)
        {
            _context = context;
        }

        // GET: Areas
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["LocationSortParm"] = sortOrder == "Location" ? "location_desc" : "Location";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var areas = from s in _context.Areas
                        .Include(d => d.Location)
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                areas = areas.Where(s => s.Name.Contains(searchString)
                            || s.Location.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    areas = areas.OrderByDescending(s => s.Name);
                    break;
                case "Location":
                    areas = areas.OrderBy(s => s.Location.Name);
                    break;
                case "location_desk":
                    areas = areas.OrderByDescending(s => s.Location.Name);
                    break;
                default:
                    areas = areas.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 15;
            return View(await PaginatedList<Area>.CreateAsync(areas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Areas/Create
        public IActionResult Create()
        {
            ViewData["Locations"] = new SelectList(_context.Locations.Where(l => l.Active == true), "ID", "Name");
            return View();
        }

        // POST: Areas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LocationID,Name,Active")] Area area)
        {
            if (ModelState.IsValid)
            {
                _context.Add(area);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Locations"] = new SelectList(_context.Locations.Where(l => l.Active == true), "ID", "Name", area.LocationID);
            return View(area);
        }

        // GET: Areas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            ViewData["Locations"] = new SelectList(_context.Locations.Where(l => l.Active == true), "ID", "Name", area.LocationID);
            return View(area);
        }

        // POST: Areas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LocationID,Name,Active")] Area area)
        {
            if (id != area.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el área existente antes de la actualización
                    var existingArea = await _context.Areas.AsNoTracking().FirstOrDefaultAsync(a => a.ID == id);

                    // Verificar si el LocationID ha cambiado
                    if (existingArea.LocationID != area.LocationID)
                    {
                        // Actualizar la localidad de todos los dispositivos que están asociados con esta área
                        var devices = _context.Devices.Where(d => d.AreaID == area.ID);
                        foreach (var device in devices)
                        {
                            device.LocationID = area.LocationID;
                        }

                        // Guardar los cambios en los dispositivos
                        _context.UpdateRange(devices);
                    }

                    _context.Update(area);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(area.ID))
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
            ViewData["Locations"] = new SelectList(_context.Locations, "ID", "Name", area.LocationID);
            return View(area);
        }

        private bool AreaExists(int id)
        {
            return _context.Areas.Any(e => e.ID == id);
        }
    }
}
