using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            ViewBag.Locations = new SelectList(_context.Locations.Where(l => l.Active == true), "ID", "Name");
            ViewBag.SelectedLocation = locationID;

            // Consultar los dispositivos incluyendo las relaciones de navegación
            IQueryable<Device> devices = _context.Devices
                .Include(d => d.Area)
                .Where(a => a.Area.Active == true)
                .Where(d => d.Active == true);     // Incluir la relación Area

            // Filtrar por localidad si está seleccionada
            if (locationID.HasValue)
            {
                devices = devices.Where(a => a.Active == true).Where(d => d.LocationID == locationID.Value);

                // Cargar las áreas correspondientes a la localidad seleccionada
                ViewBag.Areas = new SelectList(_context.Areas.Where(a => a.Active == true).Where(a => a.LocationID == locationID.Value), "ID", "Name");

                // Si también se selecciona un área, aplicar el filtro
                if (areaID.HasValue)
                {
                    devices = devices.Where(d => d.Active == true).Where(d => d.AreaID == areaID.Value);
                }
            }

            int pageSize = 10;
            return View(await PaginatedList<Device>.CreateAsync(devices.AsNoTracking(), pageIndex ?? 1, pageSize));
        }

        [Authorize(Roles = "Administrador, Operador")]
        // GET: Devices/List
        public async Task<IActionResult> List()
        {
            var device = await _context.Devices.ToListAsync();

            return View(device);
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


        [Authorize(Roles = "Administrador, Operador")]
        // GET: Devices/Create
        public IActionResult Create()
        {
            ViewData["Locations"] = new SelectList(_context.Locations.Where(a => a.Active == true), "ID", "Name");
            ViewData["DeviceTypes"] = new SelectList(_context.DeviceTypes.Where(a => a.Active == true), "ID", "Name");
            ViewData["RiskLevels"] = new SelectList(_context.RiskLevels.Where(a => a.Active == true), "ID", "Level");
            ViewData["PPES"] = new MultiSelectList(_context.PPEs.Where(a => a.Active == true), "ID", "Name");
            ViewData["Risks"] = new MultiSelectList(_context.Risks.Where(a => a.Active == true), "ID", "Name");
            return View();
        }

        // Metodo AJAX para devolver las areas cuando se seleccione una localidad
        public JsonResult GetAreasByLocation(int locationId)
        {
            var areas = _context.Areas
                .Where(a => a.Active == true)
                .Where(a => a.LocationID == locationId)
                .Select(a => new { a.ID, a.Name })
                .ToList();

            return Json(areas);
        }

        [Authorize(Roles = "Administrador, Operador")]
        // POST: Devices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LocationID,AreaID,DeviceTypeID,RiskLevelID,Image,Name,Model,Function,SpecificFunction,Operators,LastMaintenance,EmergencyStopImage,TypeSafetyDevice,FunctionSafetyDevice,Active,ImageFile,ImageFileES")] Device device)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileNameDevice = null;
                string uniqueFileNameEmergencyStop = null;

                // Manejar la imagen del equipo
                if (device.ImageFile != null)
                {
                    // Validar si el archivo es una imagen valida
                    var fileType = device.ImageFile.ContentType.ToLower();
                    var allowedFileTypes = new[] { "image/jpeg", "image/png", "image/jpg" };

                    if (!allowedFileTypes.Contains(fileType))
                    {
                        ModelState.AddModelError("ImageFile","Solo se permiten archivos con extensión .jpg, .jpeg o .png");
                        return View(device);
                    }
                    else
                    {
                        // Obtener las rutas de las carpetas donde se almacenaran las imagenes de los equipos
                        string uploadsFolderDevice = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/devices");

                        // Crear el nombre unico para las imagenes
                        uniqueFileNameDevice = Guid.NewGuid().ToString() + "_" + device.ImageFile.FileName;

                        // Combinar para obtener la ruta completa del archivo
                        string filePathDevice = Path.Combine(uploadsFolderDevice, uniqueFileNameDevice);

                        // Validar que las carpetas existan
                        if (!Directory.Exists(uploadsFolderDevice))
                        {
                            Directory.CreateDirectory(uploadsFolderDevice);
                        }

                        using (var fileStreamDevice = new FileStream(filePathDevice, FileMode.Create))
                        {
                            await device.ImageFile.CopyToAsync(fileStreamDevice);
                        }
                    }
                }

                // Manejar la imagen del paro de emergencias
                if (device.ImageFileES != null)
                {
                    // Validar si el archivo es una imagen valida
                    var fileType = device.ImageFileES.ContentType.ToLower();
                    var allowedFileTypes = new[] { "image/jpeg", "image/png", "image/jpg" };

                    if (!allowedFileTypes.Contains(fileType))
                    {
                        ModelState.AddModelError("ImageFileES", "Solo se permiten archivos con extensión .jpg, .jpeg o .png");
                        return View(device);
                    }
                    else
                    {
                        // Obtener las rutas de las carpetas donde se almacenaran las imagenes de los equipos y paros de emergencia
                        string uploadsFolderEmergencyStop = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/emergency_stops");

                        // Crear el nombre unico para las imagenes
                        uniqueFileNameEmergencyStop = Guid.NewGuid().ToString() + "_" + device.ImageFileES.FileName;

                        // Combinar para obtener la ruta completa del archivo
                        string filePathEmergencyStop = Path.Combine(uploadsFolderEmergencyStop, uniqueFileNameEmergencyStop);

                        if (!Directory.Exists(uploadsFolderEmergencyStop))
                        {
                            Directory.CreateDirectory(uploadsFolderEmergencyStop);
                        }


                        using (var fileStreamEmergencyStop = new FileStream(filePathEmergencyStop, FileMode.Create))
                        {
                            await device.ImageFileES.CopyToAsync(fileStreamEmergencyStop);
                        }
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
                // Guardar los datos del modelo en la base de datos
                _context.Add(device);
                var result = await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            ViewData["Locations"] = new SelectList(_context.Locations.Where(a => a.Active == true), "ID", "Name");
            ViewData["DeviceTypes"] = new SelectList(_context.DeviceTypes.Where(a => a.Active == true), "ID", "Name", device.DeviceTypeID);
            ViewData["RiskLevels"] = new SelectList(_context.RiskLevels.Where(a => a.Active == true), "ID", "Level", device.RiskLevelID);
            ViewData["PPES"] = new MultiSelectList(_context.PPEs.Where(a => a.Active == true), "ID", "Name");
            ViewData["Risks"] = new MultiSelectList(_context.Risks.Where(a => a.Active == true), "ID", "Name");
            return View(device);
        }

        [Authorize(Roles = "Administrador, Operador")]
        // GET: Devices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Incluir PPEs y Risks con el dispositivo
            var device = await _context.Devices
                .Include(d => d.PPEs)
                .Include(d => d.Risks)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (device == null)
            {
                return NotFound();
            }

            var selectedPPEs = device.PPEs.Where(a => a.Active == true).Select(p => p.ID).ToList();
            ViewBag.PPES = new MultiSelectList(_context.PPEs.Where(a => a.Active == true), "ID", "Name", selectedPPEs);

            var selectedRisks = device.Risks.Where(a => a.Active == true).Select(r => r.ID).ToList();
            ViewBag.Risks = new MultiSelectList(_context.Risks.Where(a => a.Active == true), "ID", "Name", selectedRisks);

            device.SelectedPPEs = selectedPPEs;
            device.SelectedRisks = selectedRisks;

            Console.WriteLine("PPEs seleccionados: " + string.Join(", ", device.PPEs.Where(p => p.Active == true).Select(p => p.ID)));
            Console.WriteLine("Risks seleccionados: " + string.Join(", ", device.Risks.Where(r => r.Active == true).Select(r => r.ID)));

            ViewData["Locations"] = new SelectList(_context.Locations.Where(a => a.Active == true), "ID", "Name", device.LocationID);
            ViewData["Areas"] = new SelectList(_context.Areas.Where(a => a.Active == true).Where(a => a.LocationID == device.LocationID), "ID", "Name", device.AreaID);
            ViewData["DeviceTypes"] = new SelectList(_context.DeviceTypes.Where(a => a.Active == true), "ID", "Name", device.DeviceTypeID);
            ViewData["RiskLevels"] = new SelectList(_context.RiskLevels.Where(a => a.Active == true), "ID", "Level", device.RiskLevelID);

            return View(device);
        }

        [Authorize(Roles = "Administrador, Operador")]
        // POST: Devices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LocationID,AreaID,DeviceTypeID,RiskLevelID,Image,Name,Model,Function,SpecificFunction,Operators,LastMaintenance,EmergencyStopImage,TypeSafetyDevice,FunctionSafetyDevice,Active,ImageFile,ImageFileES")] Device device)
        {
            if (id != device.ID)
            {
                return NotFound();
            }

            var existingDevice = await _context.Devices
                    .Include(d => d.PPEs) // Incluye la colección de PPEs
                    .Include(d => d.Risks) // Incluye la colección de riesgos
                    .FirstOrDefaultAsync(d => d.ID == id);
            if (existingDevice == null)
            {
                return NotFound();
            }

            // Si no se sube foto nueva del equipo, conservar la que ya se tiene almacenada
            if (device.ImageFile == null)
            {
                device.Image = existingDevice.Image; // Mantener la imagen existente
            }
            else
            {
                // Obtener la ruta de la imagen anterior que sera reemplazada
                var oldImageDevicePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + existingDevice.Image);
                // Logica para manejar la nueva imagen
                string uploadsFolderDevice = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/devices/");
                string uniqueFileNameDevice = Guid.NewGuid().ToString() + "_" + device.ImageFile.FileName;
                string filePathDevice = Path.Combine(uploadsFolderDevice, uniqueFileNameDevice);

                // Validar si la carpeta de imagenes de PPE existe
                if (!Directory.Exists(uploadsFolderDevice))
                {
                    Directory.CreateDirectory(uploadsFolderDevice); // Si no existe la carpeta, la crea
                }

                // Guardar la nueva imagen
                using (var fileStream = new FileStream(filePathDevice, FileMode.Create))
                {
                    await device.ImageFile.CopyToAsync(fileStream);
                }


                // Validar que exista la imagen que se reemplazara
                if (System.IO.File.Exists(oldImageDevicePath))
                {
                    System.IO.File.Delete(oldImageDevicePath); // Si existe, elimina la imagen
                }

                // Actualizar la propiedad Image con la nueva ruta
                device.Image = "/images/devices/" + uniqueFileNameDevice;
            }

            // Si no se sube foto nueva del paro de emergencia, conservar la que ya se tiene almacenada
            if (device.ImageFileES == null)
            {
                device.EmergencyStopImage = existingDevice.EmergencyStopImage; // Mantener la imagen existente
            }
            else
            {
                // Obtener la ruta de la imagen anterior que sera reemplazada
                var oldImageEmergencyStopPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + existingDevice.EmergencyStopImage);
                // Logica para manejar la nueva imagen
                string uploadsFolderEmergencyStop = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/emergency_stops/");
                string uniqueFileNameEmergencyStop = Guid.NewGuid().ToString() + "_" + device.ImageFileES.FileName;
                string filePathEmergencyStop = Path.Combine(uploadsFolderEmergencyStop, uniqueFileNameEmergencyStop);

                // Validar si la carpeta de imagenes de PPE existe
                if (!Directory.Exists(uploadsFolderEmergencyStop))
                {
                    Directory.CreateDirectory(uploadsFolderEmergencyStop); // Si no existe la carpeta, la crea
                }

                // Guardar la nueva imagen
                using (var fileStream = new FileStream(filePathEmergencyStop, FileMode.Create))
                {
                    await device.ImageFileES.CopyToAsync(fileStream);
                }


                // Validar que exista la imagen que se reemplazara
                if (System.IO.File.Exists(oldImageEmergencyStopPath))
                {
                    System.IO.File.Delete(oldImageEmergencyStopPath); // Si existe, elimina la imagen
                }

                // Actualizar la propiedad Image con la nueva ruta
                device.EmergencyStopImage = "/images/emergency_stops/" + uniqueFileNameEmergencyStop;
            }

            // Recuperar las selecciones de PPEs y Risks desde el formulario
            var selectedPPEs = Request.Form["SelectedPPEs"].ToArray();
            var selectedRisks = Request.Form["SelectedRisks"].ToArray();

            // Obtener los PPEs y Risks actuales del dispositivo
            var currentPPEs = existingDevice.PPEs.ToList();
            var currentRisks = existingDevice.Risks.ToList();

            // Eliminar PPEs que no están seleccionados
            foreach (var currentPPE in currentPPEs)
            {
                if (!selectedPPEs.Contains(currentPPE.ID.ToString()))
                {
                    Console.WriteLine($"Removing PPE with ID {currentPPE.ID} from Device.");
                    existingDevice.PPEs.Remove(currentPPE);
                }
            }

            // Agregar nuevos PPEs seleccionados
            if (selectedPPEs != null)
            {
                Console.WriteLine($"Selected PPEs: {string.Join(", ", selectedPPEs)}");
                foreach (var ppeId in selectedPPEs)
                {

                    if (int.TryParse(ppeId, out int parsedPPEId)) // Convertir ppeId a int
                    {
                        var ppe = await _context.PPEs.FindAsync(parsedPPEId);
                        if (ppe != null && !existingDevice.PPEs.Contains(ppe))
                        {
                            Console.WriteLine($"Adding PPE with ID {parsedPPEId} to Device.");
                            existingDevice.PPEs.Add(ppe);
                        }
                        else
                        {
                            Console.WriteLine($"PPE with ID {parsedPPEId} not found or already added.");
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

            // Eliminar Risks que no están seleccionados
            foreach (var currentRisk in currentRisks)
            {
                if (!selectedRisks.Contains(currentRisk.ID.ToString()))
                {
                    Console.WriteLine($"Removing Risk with ID {currentRisk.ID} from Device.");
                    existingDevice.Risks.Remove(currentRisk);
                }
            }

            if (selectedRisks != null)
            {
                Console.WriteLine($"Selected Risks: {string.Join(", ", selectedRisks)}");
                foreach (var riskId in selectedRisks)
                {
                    if (int.TryParse(riskId, out int parsedRiskId)) // Convertir riskId a int
                    {
                        var risk = await _context.Risks.FindAsync(parsedRiskId);
                        if (risk != null && !existingDevice.Risks.Contains(risk))
                        {
                            Console.WriteLine($"Adding Risk with ID {parsedRiskId} to Device.");
                            existingDevice.Risks.Add(risk);
                        }
                        else
                        {
                            Console.WriteLine($"Risk with ID {parsedRiskId} not found or already added.");
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

            //Actualizar los campos
            existingDevice.LocationID = device.LocationID;
            existingDevice.AreaID = device.AreaID;
            existingDevice.DeviceTypeID = device.DeviceTypeID;
            existingDevice.RiskLevelID = device.RiskLevelID;
            existingDevice.Image = device.Image;
            existingDevice.Name = device.Name;
            existingDevice.Model = device.Model;
            existingDevice.Function = device.Function;
            existingDevice.SpecificFunction = device.SpecificFunction;
            existingDevice.Operators = device.Operators;
            existingDevice.LastMaintenance = device.LastMaintenance;
            existingDevice.EmergencyStopImage = device.EmergencyStopImage;
            existingDevice.TypeSafetyDevice =  device.TypeSafetyDevice;
            existingDevice.FunctionSafetyDevice = device.FunctionSafetyDevice;
            existingDevice.Active = device.Active;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(existingDevice);
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
                return RedirectToAction(nameof(List));
            }

            ViewData["Locations"] = new SelectList(_context.Locations.Where(a => a.Active == true), "ID", "Name", device.LocationID);
            ViewData["Areas"] = new SelectList(_context.Areas.Where(a => a.Active == true).Where(a => a.LocationID == device.LocationID), "ID", "Name", device.AreaID);
            ViewData["DeviceTypes"] = new SelectList(_context.DeviceTypes.Where(a => a.Active == true), "ID", "Name", device.DeviceTypeID);
            ViewData["RiskLevels"] = new SelectList(_context.RiskLevels.Where(a => a.Active == true), "ID", "Level", device.RiskLevelID);

            return View(device);
        }

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.ID == id);
        }
    }
}
