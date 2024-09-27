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
    public class DevicesController : Controller
    {
        private readonly SafetoolContext _context;

        public DevicesController(SafetoolContext context)
        {
            _context = context;
        }

        // GET: Devices
        public async Task<IActionResult> Index()
        {
            var safetoolContext = _context.Devices.Include(d => d.Area).Include(d => d.DeviceType).Include(d => d.RiskLevel);
            return View(await safetoolContext.ToListAsync());
        }

        // GET: Devices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Area)
                .Include(d => d.DeviceType)
                .Include(d => d.RiskLevel)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // GET: Devices/Create
        public IActionResult Create()
        {
            ViewData["Locations"] = new SelectList(_context.Locations, "ID", "Name");
            ViewData["DeviceTypes"] = new SelectList(_context.DeviceTypes, "ID", "Name");
            ViewData["RiskLevels"] = new SelectList(_context.RiskLevels, "ID", "Level");
            return View();
        }

        // Metodo AJAX para devolver las areas cuando se seleccione una localidad
        public JsonResult GetAreasByLocation(int locationId)
        {
            var areas = _context.Areas
                .Where(a => a.LocationID == locationId)
                .Select(a => new { a.ID, a.Name })
                .ToList();

            return Json(areas);
        }


        // POST: Devices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LocationID,AreaID,DeviceTypeID,RiskLevelID,Image,Name,Function,SpecificFunction,Operators,LastMaintenance,EmergencyStopImage,TypeSafetyDevice,FunctionSafetyDevice,Active,ImageFile,ImageFileES")] Device device)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el archivo ha sido recibido correctamente
                if (device.ImageFile == null || device.ImageFile.Length == 0)
                {
                    ModelState.AddModelError("ImageFile", "Debe subir una imagen.");
                    return View(device);
                }

                // Verificar si el archivo ha sido recibido correctamente
                if (device.ImageFileES == null || device.ImageFileES.Length == 0)
                {
                    ModelState.AddModelError("ImageFile", "Debe subir una imagen.");
                    return View(device);
                }

                string uniqueFileNameDevice = null;
                string uniqueFileNameEmergencyStop = null;

                if (device.ImageFile != null && device.ImageFileES != null)
                {
                    // Obtener las rutas de las carpetas donde se almacenaran las imagenes de los equipos y paros de emergencia
                    string uploadsFolderDevice = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/devices");
                    string uploadsFolderEmergencyStop = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/emergency_stops");

                    // Crear el nombre unico para las imagenes
                    uniqueFileNameDevice = Guid.NewGuid().ToString() + "_" + device.ImageFile.FileName;
                    uniqueFileNameEmergencyStop = Guid.NewGuid().ToString() + "_" + device.ImageFileES.FileName;

                    // Combinar para obtener la ruta completa del archivo
                    string filePathDevice = Path.Combine(uploadsFolderDevice, uniqueFileNameDevice);
                    string filePathEmergencyStop = Path.Combine(uploadsFolderEmergencyStop, uniqueFileNameEmergencyStop);

                    // Validar que las carpetas existan
                    if (!Directory.Exists(uploadsFolderDevice))
                    {
                        Directory.CreateDirectory(uploadsFolderDevice);
                    }

                    if (!Directory.Exists(uploadsFolderEmergencyStop))
                    {
                        Directory.CreateDirectory(uploadsFolderEmergencyStop);
                    }

                    using (var fileStreamDevice = new FileStream(filePathDevice, FileMode.Create))
                    {
                        await device.ImageFile.CopyToAsync(fileStreamDevice);
                    }

                    using (var fileStreamEmergencyStop = new FileStream(filePathEmergencyStop, FileMode.Create))
                    {
                        await device.ImageFileES.CopyToAsync(fileStreamEmergencyStop);
                    }
                }

                if (!string.IsNullOrEmpty(uniqueFileNameDevice))
                {
                    // Asignar el nombre de la imagen al objeto existente
                    device.Image = "/images/devices/" + uniqueFileNameDevice;
                }

                if (!string.IsNullOrEmpty(uniqueFileNameEmergencyStop))
                {
                    // Asignar el nombre de la imagen al objeto existente
                    device.EmergencyStopImage = "/images/emergency_stops/" + uniqueFileNameEmergencyStop;
                }

                // Guardar los datos del modelo en la base de datos
                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Locations"] = new SelectList(_context.Locations, "ID", "Name");
            ViewData["DeviceTypeID"] = new SelectList(_context.DeviceTypes, "ID", "Name", device.DeviceTypeID);
            ViewData["RiskLevelID"] = new SelectList(_context.RiskLevels, "ID", "Level", device.RiskLevelID);
            return View(device);
        }

        // GET: Devices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            ViewData["AreaID"] = new SelectList(_context.Areas, "ID", "ID", device.AreaID);
            ViewData["DeviceTypeID"] = new SelectList(_context.DeviceTypes, "ID", "ID", device.DeviceTypeID);
            ViewData["RiskLevelID"] = new SelectList(_context.RiskLevels, "ID", "ID", device.RiskLevelID);
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LocationID,AreaID,DeviceTypeID,RiskLevelID,Image,Name,Function,SpecificFunction,Operators,LastMaintenance,EmergencyStopImage,TypeSafetyDevice,FunctionSafetyDevice,Active")] Device device)
        {
            if (id != device.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.ID))
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
            ViewData["AreaID"] = new SelectList(_context.Areas, "ID", "ID", device.AreaID);
            ViewData["DeviceTypeID"] = new SelectList(_context.DeviceTypes, "ID", "ID", device.DeviceTypeID);
            ViewData["RiskLevelID"] = new SelectList(_context.RiskLevels, "ID", "ID", device.RiskLevelID);
            return View(device);
        }

        // GET: Devices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Area)
                .Include(d => d.DeviceType)
                .Include(d => d.RiskLevel)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.ID == id);
        }
    }
}
