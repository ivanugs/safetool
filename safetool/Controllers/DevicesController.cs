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
            ViewData["AreaID"] = new SelectList(_context.Areas, "ID", "ID");
            ViewData["DeviceTypeID"] = new SelectList(_context.DeviceTypes, "ID", "ID");
            ViewData["RiskLevelID"] = new SelectList(_context.RiskLevels, "ID", "ID");
            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LocationID,AreaID,DeviceTypeID,RiskLevelID,Image,Name,Function,SpecificFunction,Operators,LastMaintenance,EmergencyStopImage,TypeSafetyDevice,FunctionSafetyDevice,Active")] Device device)
        {
            if (ModelState.IsValid)
            {
                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaID"] = new SelectList(_context.Areas, "ID", "ID", device.AreaID);
            ViewData["DeviceTypeID"] = new SelectList(_context.DeviceTypes, "ID", "ID", device.DeviceTypeID);
            ViewData["RiskLevelID"] = new SelectList(_context.RiskLevels, "ID", "ID", device.RiskLevelID);
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
