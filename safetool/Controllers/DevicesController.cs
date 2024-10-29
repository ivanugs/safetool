using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using safetool.Data;
using safetool.Models;
using safetool.Services;

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

        public async Task<IActionResult> GetTotalDevices()
        {
            int totalDevices = 0;
            totalDevices = await _context.Devices.CountAsync();

            return Json(new { totalDevices });
        }

        public bool IsRegistered(int deviceID, string employeeUID)
        {
            // Obtener la última fecha de registro
            var lastSubmission = _context.FormSubmissions
                .Where(f => f.DeviceID == deviceID && f.EmployeeUID == employeeUID)
                .OrderByDescending(f => f.CreatedAt)
                .FirstOrDefault();

            // Si no hay registros previos, no está registrado
            if (lastSubmission == null)
            {
                return false;
            }

            // Verificar si han pasado más de 6 meses desde la fecha de registro
            if (lastSubmission.CreatedAt.AddMonths(6) <= DateTime.Now)
            {
                return false; // Registro ha expirado
            }

            return true; // Registro válido
        }


        // GET: Devices
        public async Task<IActionResult> Index(int? locationID, int? areaID, int? pageIndex)
        {
            var userId = User.Identity.Name;  // Obtiene el UID del usuario autenticado

            // Obtener la lista de localidades para el dropdown
            ViewBag.Locations = new SelectList(_context.Locations.Where(l => l.Active == true), "ID", "Name");
            ViewBag.SelectedLocation = locationID;

            // Consultar los dispositivos incluyendo las relaciones de navegación
            IQueryable<Device> devices = _context.Devices
                .Include(d => d.Area)
                .Include(d => d.Area.Location)
                .Where(d => d.Area.Location.Active == true)
                .Where(d => d.Area.Active == true)
                .Where(d => d.Active == true);

            // Filtrar por localidad si está seleccionada
            if (locationID.HasValue)
            {
                devices = devices.Where(a => a.Active == true).Where(d => d.Area.Active == true).Where(d => d.LocationID == locationID.Value);

                // Cargar las áreas correspondientes a la localidad seleccionada
                ViewBag.Areas = new SelectList(_context.Areas.Where(a => a.Active == true).Where(a => a.LocationID == locationID.Value), "ID", "Name");

                // Si también se selecciona un área, aplicar el filtro
                if (areaID.HasValue)
                {
                    devices = devices.Where(d => d.Active == true).Where(d => d.Area.Active == true).Where(d => d.AreaID == areaID.Value);
                }
            }

            // Verificar si el usuario ya ha registrado algún dispositivo
            var registeredDevices = new List<int>();

            foreach (var device in devices)
            {
                if (IsRegistered(device.ID, userId))
                {
                    registeredDevices.Add(device.ID);  // Añadir el ID si está registrado y no ha expirado
                }
            }

            ViewBag.RegisteredDevices = registeredDevices;

            int pageSize = 10;
            ViewBag.TotalDevices = await devices.CountAsync();
            return View(await PaginatedList<Device>.CreateAsync(devices.AsNoTracking(), pageIndex ?? 1, pageSize));
        }

        [Authorize(Roles = "Administrador, Operador")]
        // GET: Devices/List
        public async Task<IActionResult> List(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["ModelSortParm"] = sortOrder == "Model" ? "model_desc" : "Model";
            ViewData["AreaSortParm"] = sortOrder == "Area" ? "area_desc" : "Area";
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

            var devices = from s in _context.Devices
                          .Include(d => d.Area)
                          .Include(d => d.DeviceType)
                          .Include(d => d.RiskLevel)
                          .Include(d => d.PPEs)
                          .Include(d => d.Risks)
                          .Include(d => d.Area.Location)
                          select s;

            if (!String.IsNullOrEmpty(searchString))        
            {
                devices = devices.Where(s => s.Name.Contains(searchString)
                                  || s.Area.Name.Contains(searchString)
                                  || s.Area.Location.Name.Contains(searchString)
                                  || s.Model.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    devices = devices.OrderByDescending(s => s.Name);
                    break;
                case "Location":
                    devices = devices.OrderBy(s => s.Area.Location.Name);
                    break;
                case "location_desc":
                    devices = devices.OrderByDescending(s => s.Area.Location.Name);
                    break;
                case "Area":
                    devices = devices.OrderBy(s => s.Area.Name);
                    break;
                case "area_desc":
                    devices = devices.OrderByDescending(s => s.Area.Name);
                    break;
                case "Model":
                    devices = devices.OrderBy(s => s.Model);
                    break;
                case "model_desc":
                    devices = devices.OrderByDescending(s => s.Model);
                    break;
                default:
                    devices = devices.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 15;
            ViewBag.TotalDevices = await devices.CountAsync();
            return View(await PaginatedList<Device>.CreateAsync(devices.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        // GET: Devices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

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

            // Obtener el UID del usuario autenticado
            var userId = User.Identity.Name;

            // Verificar si el dispositivo ya ha sido registrado por el usuario
            var isRegistered = IsRegistered(device.ID, userId);

            // Pasar la información de registro a la vista
            ViewBag.IsRegistered = isRegistered;

            return View(device);
        }


        [Authorize(Roles = "Administrador, Operador")]
        // GET: Devices/Create
        public IActionResult Create()
        {
            var device = new Device
            {
                LastMaintenance = DateOnly.FromDateTime(DateTime.Today) // Establecer la fecha actual
            };
            PopulateViewData(device);
            return View(device);
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
                // Verificar si el archivo ha sido recibido correctamente
                if (device.ImageFile == null || device.ImageFile.Length == 0)
                {
                    ModelState.AddModelError("ImageFile", "Debe subir una imagen.");
                    // Llamar a un método para llenar los ViewData antes de retornar
                    PopulateViewData(device);
                    return View(device);
                }

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
                        PopulateViewData(device);
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
                        PopulateViewData(device);
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
                    foreach (var ppeId in selectedPPEs)
                    {
                        if (int.TryParse(ppeId, out int parsedPPEId))
                        {
                            var ppe = await _context.PPEs.FindAsync(parsedPPEId);
                            if (ppe != null)
                            {
                                device.PPEs.Add(ppe);
                            }
                        }
                    }
                }

                if (selectedRisks != null && selectedRisks.Any())
                {
                    foreach (var riskId in selectedRisks)
                    {
                        if (int.TryParse(riskId, out int parsedRiskId))
                        {
                            var risk = await _context.Risks.FindAsync(parsedRiskId);
                            if (risk != null)
                            {
                                device.Risks.Add(risk);
                            }
                        }
                    }
                }

                // Guardar los datos del modelo en la base de datos
                _context.Add(device);
                var result = await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            PopulateViewData(device);
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

            // Llamar a la función para poblar ViewData y ViewBag
            PopulateDeviceData(device);

            // Almacenar los PPEs y Risks seleccionados
            device.SelectedPPEs = device.PPEs.Where(a => a.Active == true).Select(p => p.ID).ToList();
            device.SelectedRisks = device.Risks.Where(a => a.Active == true).Select(r => r.ID).ToList();

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
            if (device.ImageFile != null)
            {
                // Validar si el archivo es una imagen valida
                var fileType = device.ImageFile.ContentType.ToLower();
                var allowedFileTypes = new[] { "image/jpeg", "image/png", "image/jpg" };

                if (!allowedFileTypes.Contains(fileType))
                {
                    ModelState.AddModelError("ImageFile", "Solo se permiten archivos con extensión .jpg, .jpeg o .png");
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
            }
            else
            {
                device.Image = existingDevice.Image; // Mantener la imagen existente
            }

            // Si no se sube foto nueva del paro de emergencia, conservar la que ya se tiene almacenada
            if (device.ImageFileES != null)
            {
                // Validar si el archivo es una imagen valida
                var fileType = device.ImageFileES.ContentType.ToLower();
                var allowedFileTypes = new[] { "image/jpeg", "image/png", "image/jpg" };

                if (!allowedFileTypes.Contains(fileType))
                {
                    ModelState.AddModelError("ImageFileES", "Solo se permiten archivos con extensión .jpg, .jpeg o .png");
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
            }
            else
            {
                device.EmergencyStopImage = existingDevice.EmergencyStopImage; // Mantener la imagen existente
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
                    existingDevice.PPEs.Remove(currentPPE);
                }
            }

            // Agregar nuevos PPEs seleccionados
            if (selectedPPEs != null)
            {
                foreach (var ppeId in selectedPPEs)
                {

                    if (int.TryParse(ppeId, out int parsedPPEId)) // Convertir ppeId a int
                    {
                        var ppe = await _context.PPEs.FindAsync(parsedPPEId);
                        if (ppe != null && !existingDevice.PPEs.Contains(ppe))
                        {
                            existingDevice.PPEs.Add(ppe);
                        }
                    }
                }
            }

            // Eliminar Risks que no están seleccionados
            foreach (var currentRisk in currentRisks)
            {
                if (!selectedRisks.Contains(currentRisk.ID.ToString()))
                {
                    existingDevice.Risks.Remove(currentRisk);
                }
            }

            if (selectedRisks != null)
            {
                foreach (var riskId in selectedRisks)
                {
                    if (int.TryParse(riskId, out int parsedRiskId)) // Convertir riskId a int
                    {
                        var risk = await _context.Risks.FindAsync(parsedRiskId);
                        if (risk != null && !existingDevice.Risks.Contains(risk))
                        {
                            existingDevice.Risks.Add(risk);
                        }
                    }
                }
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

            // Si hay errores de validación, mantener los PPEs y Risks seleccionados
            PopulateDeviceData(existingDevice);

            // Almacenar los PPEs y Risks seleccionados
            existingDevice.SelectedPPEs = existingDevice.PPEs.Where(a => a.Active).Select(p => p.ID).ToList();
            existingDevice.SelectedRisks = existingDevice.Risks.Where(a => a.Active).Select(r => r.ID).ToList();

            return View(existingDevice);
        }

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.ID == id);
        }

        private void PopulateViewData(Device device)
        {
            ViewData["Locations"] = new SelectList(_context.Locations.Where(a => a.Active == true), "ID", "Name", device.LocationID);
            ViewData["DeviceTypes"] = new SelectList(_context.DeviceTypes.Where(a => a.Active == true), "ID", "Name", device.DeviceTypeID);
            ViewData["RiskLevels"] = new SelectList(_context.RiskLevels.Where(a => a.Active == true), "ID", "Level", device.RiskLevelID);
            ViewData["PPES"] = new MultiSelectList(_context.PPEs.Where(a => a.Active == true), "ID", "Name");
            ViewData["Risks"] = new MultiSelectList(_context.Risks.Where(a => a.Active == true), "ID", "Name");

            // Llenar áreas según la ubicación seleccionada
            if (device.LocationID != null)
            {
                var areas = _context.Areas.Where(a => a.LocationID == device.LocationID && a.Active).ToList();
                ViewData["Areas"] = new SelectList(areas, "ID", "Name", device.AreaID);
            }
            else
            {
                ViewData["Areas"] = new SelectList(Enumerable.Empty<Area>(), "ID", "Name");
            }
        }

        private void PopulateDeviceData(Device device)
        {
            // Llenar Locations
            ViewData["Locations"] = new SelectList(_context.Locations.Where(a => a.Active), "ID", "Name", device.LocationID);

            // Llenar DeviceTypes
            ViewData["DeviceTypes"] = new SelectList(_context.DeviceTypes.Where(a => a.Active), "ID", "Name", device.DeviceTypeID);

            // Llenar RiskLevels
            ViewData["RiskLevels"] = new SelectList(_context.RiskLevels.Where(a => a.Active), "ID", "Level", device.RiskLevelID);

            // Llenar PPEs
            var selectedPPEs = device.PPEs.Where(a => a.Active).Select(p => p.ID).ToList();
            ViewBag.PPES = new MultiSelectList(_context.PPEs.Where(a => a.Active), "ID", "Name", selectedPPEs);

            // Llenar Risks
            var selectedRisks = device.Risks.Where(a => a.Active).Select(r => r.ID).ToList();
            ViewBag.Risks = new MultiSelectList(_context.Risks.Where(a => a.Active), "ID", "Name", selectedRisks);

            // Llenar áreas según la ubicación seleccionada
            if (device.LocationID != null)
            {
                var areas = _context.Areas.Where(a => a.LocationID == device.LocationID && a.Active).ToList();
                ViewData["Areas"] = new SelectList(areas, "ID", "Name", device.AreaID);
            }
            else
            {
                ViewData["Areas"] = new SelectList(Enumerable.Empty<Area>(), "ID", "Name");
            }
        }

    }
}
