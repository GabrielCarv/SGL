using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Library.Data;
using MVC_Library.Models;

namespace MVC_Library.Controllers
{
    public class BooksController : Controller
    {
        private readonly MVC_LibraryContext _context;


        public IEnumerable<string> Categories { get; private set; }

        public BooksController(MVC_LibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public IActionResult Index()
        {
            List<Book> mVC_LibraryContext = _context.Books.Include(b => b.PublishingCompany).ToList();
            List<BookCategoriesIndexViewModel> bookCategories = new List<BookCategoriesIndexViewModel>();

            foreach (Book book in mVC_LibraryContext)
            {
                List<Category> categories = _context.BookCategories.Where(a => a.BookId == book.BookId).Select(a => a.Category).ToList();
                BookCategoriesIndexViewModel bookCategoriesIndex = new BookCategoriesIndexViewModel { Book = book, Categories = categories };
                bookCategories.Add(bookCategoriesIndex);
            }

            return View(bookCategories.ToList());

        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book book = await _context.Books
                .Include(b => b.PublishingCompany)
                .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            List<Category> categories = _context.BookCategories.Where(a => a.BookId == book.BookId).Select(a => a.Category).ToList();
            BookCategoriesIndexViewModel bookCategoriesIndex = new BookCategoriesIndexViewModel { Book = book, Categories = categories };

            return View(bookCategoriesIndex);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["PublishingCompanyId"] = new SelectList(_context.PublishingCompanies, "PublishingCompanyId", "PublishingCompanyName");
            ViewBag.CategoryId = new MultiSelectList(_context.Categories.ToList(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("BookId,Title,Author,PublishDate,BasePrice,Quantity,PublishingCompanyId,CategoryId")] Book book)
        {
            if (ModelState.IsValid)
            {
                if(BeUniqueBook(book.Title))
                {

                    using var transaction = _context.Database.BeginTransaction();

                    try
                    {
                        transaction.CreateSavepoint("BeforeNewBook");
                        _context.Add(book);
                        _context.SaveChanges();

                        List<string> categoryIds = Request.Form["CategoryId"].ToList();

                        foreach (string id in categoryIds)
                        {
                            BookCategory bookCategory = new BookCategory { CategoryId = Convert.ToInt32(id), BookId = book.BookId };
                            _context.BookCategories.Add(bookCategory);
                            _context.SaveChanges();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        try
                        {
                            transaction.RollbackToSavepoint("BeforeNewBook");
                        }
                        catch (Exception ex)
                        {
                            // This catch block will handle any errors that may have occurred
                            // on the server that would cause the rollback to fail, such as
                            // a closed connection.
                            Console.WriteLine("Rollback Exception Type: {0}", ex.GetType());
                            Console.WriteLine("  Message: {0}", ex.Message);
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Book Title Already Exist!");
                }
                
            }

            ViewData["PublishingCompanyId"] = new SelectList(_context.PublishingCompanies, "PublishingCompanyId", "PublishingCompanyName", book.PublishingCompanyId);
            ViewBag.CategoryId = new MultiSelectList(_context.Categories.ToList(), "CategoryId", "CategoryName");
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            BookCategoriesEditViewModel bookCategoriesViewModel = new BookCategoriesEditViewModel { Book = book, Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName"), SelectedCategories = _context.BookCategories.Where(e => e.BookId == id).Select(e => e.CategoryId).ToList() };
            ViewData["PublishingCompanyId"] = new SelectList(_context.PublishingCompanies, "PublishingCompanyId", "PublishingCompanyName", book.PublishingCompanyId);

            return View(bookCategoriesViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("BookId,Title,Author,PublishDate,BasePrice,Quantity,PublishingCompanyId,CategoryId")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (BeUniqueBook(book.Title))
                {
                    using var transaction = _context.Database.BeginTransaction();
                    try
                    {
                        transaction.CreateSavepoint("BeforeEditBook");

                        List<string> categoryIds = Request.Form["SelectedCategories"].ToList();

                        string publishDate = ("01/01/" + Request.Form["Book.PublishDate.Year"]);
                        book.PublishDate = Convert.ToDateTime(publishDate);

                        _context.Books.Update(book);
                        _context.SaveChanges();

                        List<BookCategory> bookCategoriesFromDB = _context.BookCategories.Where(a => a.BookId == book.BookId).ToList();
                        List<BookCategory> bookCategoriesFromView = new List<BookCategory>();
                        List<BookCategory> bookCategoriesFromDBCheck = new List<BookCategory>();

                        bookCategoriesFromDBCheck.AddRange(bookCategoriesFromDB);

                        foreach (string idCategoryFromView in categoryIds)
                        {
                            BookCategory bookCategory = new BookCategory { Book = book, CategoryId = Convert.ToInt32(idCategoryFromView), BookId = book.BookId };
                            bookCategoriesFromView.Add(bookCategory);
                        }

                        foreach (BookCategory bookCategory in bookCategoriesFromDBCheck)
                        {
                            bool exist = bookCategoriesFromView.Any(a => a.CategoryId == bookCategory.CategoryId);
                            if (exist)
                            {
                                bookCategoriesFromView.Remove(bookCategoriesFromView.Where(a => a.CategoryId == bookCategory.CategoryId).First());
                                bookCategoriesFromDB.Remove(bookCategory);
                            }
                        }

                        _context.BulkDelete(bookCategoriesFromDB);
                        _context.BulkInsert(bookCategoriesFromView);
                        _context.SaveChanges();

                        transaction.Commit();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BookExists(book.BookId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            try
                            {
                                transaction.RollbackToSavepoint("BeforeEditBook");
                            }
                            catch (Exception ex)
                            {
                                // This catch block will handle any errors that may have occurred
                                // on the server that would cause the rollback to fail, such as
                                // a closed connection.
                                Console.WriteLine("Rollback Exception Type: {0}", ex.GetType());
                                Console.WriteLine("  Message: {0}", ex.Message);
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Book Title Already Exist!");
                }
            }

            BookCategoriesEditViewModel bookCategoriesViewModel = new BookCategoriesEditViewModel { Book = book, Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName"), SelectedCategories = _context.BookCategories.Where(e => e.BookId == book.BookId).Select(e => e.CategoryId).ToList() };
            ViewData["PublishingCompanyId"] = new SelectList(_context.PublishingCompanies, "PublishingCompanyId", "PublishingCompanyName", book.PublishingCompanyId);

            return View(bookCategoriesViewModel);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.PublishingCompany).Include(c => c.BookCategories)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);

        }

        private bool BeUniqueBook(string title)
        {
            return _context.Books.Any(x => x.Title == title);
        }

    }

}
