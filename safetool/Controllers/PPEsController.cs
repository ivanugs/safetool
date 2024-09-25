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
    public class PPEsController : Controller
    {
        private readonly SafetoolContext _context;

        public PPEsController(SafetoolContext context)
        {
            _context = context;
        }

        // GET: PPEs
        public async Task<IActionResult> Index()
        {
            return View(await _context.PPEs.ToListAsync());
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Image,Active")] PPE pPE)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pPE);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Image,Active")] PPE pPE)
        {
            if (id != pPE.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pPE);
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

        // GET: PPEs/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: PPEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pPE = await _context.PPEs.FindAsync(id);
            if (pPE != null)
            {
                _context.PPEs.Remove(pPE);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PPEExists(int id)
        {
            return _context.PPEs.Any(e => e.ID == id);
        }
    }
}
