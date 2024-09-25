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
    public class UserRolesController : Controller
    {
        private readonly SafetoolContext _context;

        public UserRolesController(SafetoolContext context)
        {
            _context = context;
        }

        // GET: UserRoles
        public async Task<IActionResult> Index()
        {
            var safetoolContext = _context.UserRoles.Include(u => u.Role);
            return View(await safetoolContext.ToListAsync());
        }

        // GET: UserRoles/Create
        public IActionResult Create()
        {
            ViewData["Roles"] = new SelectList(_context.Roles, "ID", "Name");
            return View();
        }

        // POST: UserRoles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserName,RoleID")] UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Si la validación falla, revisa los errores en el ModelState
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            ViewData["Roles"] = new SelectList(_context.Roles, "ID", "Name", userRole.RoleID);
            return View(userRole);
        }

        // GET: UserRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }
            ViewData["Roles"] = new SelectList(_context.Roles, "ID", "Name", userRole.RoleID);
            return View(userRole);
        }

        // POST: UserRoles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserName,RoleID")] UserRole userRole)
        {
            if (id != userRole.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserRoleExists(userRole.ID))
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
            ViewData["Roles"] = new SelectList(_context.Roles, "ID", "Name", userRole.RoleID);
            return View(userRole);
        }

        private bool UserRoleExists(int id)
        {
            return _context.UserRoles.Any(e => e.ID == id);
        }
    }
}
