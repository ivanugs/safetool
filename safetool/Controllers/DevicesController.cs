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
        private readonly IConfiguration _configuration;

        public DevicesController(SafetoolContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Devices
        public async Task<IActionResult> Index(int? locationID, int? areaID, int? pageIndex)
        {
            // Obtener la lista de localidades para el dropdown
            ViewBag.Locations = new SelectList(_context.Locations, "ID", "Name");
            ViewBag.SelectedLocation = locationID;

            // Consultar los dispositivos incluyendo las relaciones de navegación
            IQueryable<Device> devices = _context.Devices
                .Include(d => d.Area);     // Incluir la relación Area

            // Filtrar por localidad si está seleccionada
            if (locationID.HasValue)
            {
                devices = devices.Where(d => d.LocationID == locationID.Value);

                // Cargar las áreas correspondientes a la localidad seleccionada
                ViewBag.Areas = new SelectList(_context.Areas.Where(a => a.LocationID == locationID.Value), "ID", "Name");

                // Si también se selecciona un área, aplicar el filtro
                if (areaID.HasValue)
                {
                    devices = devices.Where(d => d.AreaID == areaID.Value);
                }
            }

            int pageSize = 10;
            return View(await PaginatedList<Device>.CreateAsync(devices.AsNoTracking(), pageIndex ?? 1, pageSize));
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
                .Include(d => d.PPEs)
                .Include(d => d.Risks)
                .Include(d => d.Area.Location)
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
            ViewData["PPES"] = new MultiSelectList(_context.PPEs, "ID", "Name");
            ViewData["Risks"] = new MultiSelectList(_context.Risks, "ID", "Name");
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
        public async Task<IActionResult> Create([Bind("ID,LocationID,AreaID,DeviceTypeID,RiskLevelID,Image,Name,Model,Function,SpecificFunction,Operators,LastMaintenance,EmergencyStopImage,TypeSafetyDevice,FunctionSafetyDevice,Active,ImageFile,ImageFileES")] Device device)
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

                var selectedPPEs = Request.Form["PPEs"].ToArray();
                var selectedRisks = Request.Form["Risks"].ToArray();

                if (selectedPPEs != null && selectedPPEs.Any())
                {
                    Console.WriteLine($"Selected PPEs: {string.Join(", ", selectedPPEs)}");
                    foreach (var ppeId in selectedPPEs)
                    {
                        if (int.TryParse(ppeId, out int parsedPPEId)) // Convertir ppeId a int
                        {
                            var ppe = await _context.PPEs.FindAsync(parsedPPEId);
                            if (ppe != null)
                            {
                                Console.WriteLine($"Adding PPE with ID {parsedPPEId} to Device.");
                                device.PPEs.Add(ppe);
                            }
                            else
                            {
                                Console.WriteLine($"PPE with ID {parsedPPEId} not found.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Invalid PPE ID: {ppeId}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No PPEs selected.");
                }

                if (selectedRisks != null && selectedRisks.Any())
                {
                    Console.WriteLine($"Selected Risks: {string.Join(", ", selectedRisks)}");
                    foreach (var riskId in selectedRisks)
                    {
                        if (int.TryParse(riskId, out int parsedRiskId)) // Convertir riskId a int
                        {
                            var risk = await _context.Risks.FindAsync(parsedRiskId);
                            if (risk != null)
                            {
                                Console.WriteLine($"Adding Risk with ID {parsedRiskId} to Device.");
                                device.Risks.Add(risk);
                            }
                            else
                            {
                                Console.WriteLine($"Risk with ID {parsedRiskId} not found.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Invalid Risk ID: {riskId}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No Risks selected.");
                }

                // Verifica la cantidad de PPEs y Risks asociados
                Console.WriteLine($"Device has {device.PPEs.Count} PPEs and {device.Risks.Count} Risks.");



                // Guardar los datos del modelo en la base de datos
                _context.Add(device);
                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"SaveChanges result: {result}"); // Esto debería devolver el número de registros afectados

                // Verificar el estado de las entidades
                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["Locations"] = new SelectList(_context.Locations, "ID", "Name");
            ViewData["DeviceTypeID"] = new SelectList(_context.DeviceTypes, "ID", "Name", device.DeviceTypeID);
            ViewData["RiskLevelID"] = new SelectList(_context.RiskLevels, "ID", "Level", device.RiskLevelID);
            ViewData["PPES"] = new MultiSelectList(_context.PPEs, "ID", "Name");
            ViewData["Risks"] = new MultiSelectList(_context.Risks, "ID", "Name");
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
            ViewData["Locations"] = new SelectList(_context.Locations, "ID", "Name");
            ViewData["Areas"] = new SelectList(_context.Areas, "ID", "Name", device.AreaID);
            ViewData["DeviceTypes"] = new SelectList(_context.DeviceTypes, "ID", "Name", device.DeviceTypeID);
            ViewData["RiskLevels"] = new SelectList(_context.RiskLevels, "ID", "Level", device.RiskLevelID);
            ViewData["PPES"] = new MultiSelectList(_context.PPEs, "ID", "Name");
            ViewData["Risks"] = new MultiSelectList(_context.Risks, "ID", "Name");
            return View(device);
        }

        // POST: Devices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LocationID,AreaID,DeviceTypeID,RiskLevelID,Image,Name,Model,Function,SpecificFunction,Operators,LastMaintenance,EmergencyStopImage,TypeSafetyDevice,FunctionSafetyDevice,Active")] Device device)
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
