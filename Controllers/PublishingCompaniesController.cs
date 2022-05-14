using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Library.Data;
using MVC_Library.Models;

namespace MVC_Library.Controllers
{
    public class PublishingCompaniesController : Controller
    {
        private readonly MVC_LibraryContext _context;

        public PublishingCompaniesController(MVC_LibraryContext context)
        {
            _context = context;
        }

        // GET: PublishingCompanies
        public async Task<IActionResult> Index()
        {
            return View(await _context.PublishingCompanies.ToListAsync());
        }

        // GET: PublishingCompanies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publishingCompany = await _context.PublishingCompanies
                .FirstOrDefaultAsync(m => m.PublishingCompanyId == id);
            if (publishingCompany == null)
            {
                return NotFound();
            }

            return View(publishingCompany);
        }

        // GET: PublishingCompanies/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PublishingCompanyId,PublishingCompanyName")] PublishingCompany publishingCompany)
        {
            if (ModelState.IsValid)
            {
                if (!PublishingCompanyExists(publishingCompany.PublishingCompanyId))
                {
                    _context.Add(publishingCompany);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Publishing Company Name Already Exist!");
                }
            }

            return View(publishingCompany);
        }

        // GET: PublishingCompanies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publishingCompany = await _context.PublishingCompanies.FindAsync(id);
            if (publishingCompany == null)
            {
                return NotFound();
            }
            return View(publishingCompany);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PublishingCompanyId,PublishingCompanyName")] PublishingCompany publishingCompany)
        {
            if (id != publishingCompany.PublishingCompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!PublishingCompanyExists(id))
                {
                    try
                    {
                        _context.Update(publishingCompany);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PublishingCompanyExists(publishingCompany.PublishingCompanyId))
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
                else
                {
                    ModelState.AddModelError("", "Publishing Company Name Already Exist!");
                }
            }

            return View(publishingCompany);
        }

        // GET: PublishingCompanies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publishingCompany = await _context.PublishingCompanies
                .FirstOrDefaultAsync(m => m.PublishingCompanyId == id);
            if (publishingCompany == null)
            {
                return NotFound();
            }

            return View(publishingCompany);
        }

        // POST: PublishingCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publishingCompany = await _context.PublishingCompanies.FindAsync(id);
            _context.PublishingCompanies.Remove(publishingCompany);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublishingCompanyExists(int id)
        {
            return _context.PublishingCompanies.Any(e => e.PublishingCompanyId == id);
        }


    }
}
