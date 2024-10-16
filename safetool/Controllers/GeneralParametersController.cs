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
    [Authorize(Roles = "Administrador")]
    public class GeneralParametersController : Controller
    {
        private readonly SafetoolContext _context;

        public GeneralParametersController(SafetoolContext context)
        {
            _context = context;
        }

        // GET: GeneralParameters
        public async Task<IActionResult> Index()
        {
            return View(await _context.GeneralParameters.ToListAsync());
        }

        // GET: GeneralParameters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalParameter = await _context.GeneralParameters.FindAsync(id);
            if (generalParameter == null)
            {
                return NotFound();
            }
            return View(generalParameter);
        }

        // POST: GeneralParameters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmailAccount,EmailAccountDisplayName,EmailAccountPassword,EmailAccountUser,EmailPort,EmailServer,EmailSsl")] GeneralParameter generalParameter)
        {
            if (id != generalParameter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(generalParameter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneralParameterExists(generalParameter.Id))
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
            return View(generalParameter);
        }

        private bool GeneralParameterExists(int id)
        {
            return _context.GeneralParameters.Any(e => e.Id == id);
        }
    }
}
