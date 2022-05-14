using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Library.Data;
using MVC_Library.Models;
using Microsoft.AspNetCore.Http;

namespace MVC_Library.Controllers
{
    
    public class PeopleController : Controller
    {
        private readonly MVC_LibraryContext _context;

        public PeopleController(MVC_LibraryContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Login(Person person)
        {
            Person personBd = _context.Peoples.Where(a => a.Cpf == person.Cpf).FirstOrDefault();

            try 
            {
                if (personBd.Password != null)
                {
                    string passwordFromLogin = string.Empty;

                    if (person.Password != null && person.Password != "")
                    {
                        passwordFromLogin = EncryptPassword(person.Cpf, person.Password);
                    }

                    bool correctPassword = correctPassword = personBd.Password.Equals(passwordFromLogin);

                    //post (login)
                    if (correctPassword)
                    {
                        HttpContext.Session.SetString("UserID", person.Cpf);
                        return Redirect("Home/Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login credentials.");
                    }
                }
            }
            catch { }

            
            return View();
        }

        // GET: People
        [CustomAuthorization]
        public IActionResult Index()
        {
            ViewBag.Message = HttpContext.Session.GetString("UserID");
            List<Person> people = _context.Peoples.ToList();
            List<PeoplePhonesViewModel> personPhones = new List<PeoplePhonesViewModel>();

            foreach (Person person in people)
            {
                List<Phone> phones = _context.Phones.Where(a => a.Person == person).ToList();
                PeoplePhonesViewModel peoplePhonesIndex = new PeoplePhonesViewModel { Person = person, Phones = phones };
                personPhones.Add(peoplePhonesIndex);
            }

            return View(personPhones.ToList());

        }

        // GET: People/Details/5
        [CustomAuthorization]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Peoples
                .FirstOrDefaultAsync(m => m.Cpf == id);

            if (person == null)
            {
                return NotFound();
            }

            List<Phone> phones = _context.Phones.Where(a => a.Person == person).ToList();
            PeoplePhonesViewModel peoplePhones = new PeoplePhonesViewModel { Person = person, Phones = phones };

            return View(peoplePhones);

        }

        [CustomAuthorization]
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }

        // GET: People/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Create([Bind("Cpf,Name,Email,BirthDate,IsLibrarian,CEP,Password,State,City,Address,HouseNumber")] Person person)
        {
            if (ModelState.IsValid)
            {
                if (!PersonExists(person.Cpf))
                {
                    if (BeUniqueEmail(person.Email))
                    {
                        using var transaction = _context.Database.BeginTransaction();

                        try
                        {
                            transaction.CreateSavepoint("NewPerson");
                            string password = person.Password;
                            person.Password = EncryptPassword(person.Cpf, password);

                            _context.Add(person);
                            _context.SaveChanges();

                            List<string> numbers = Request.Form["Number"].ToList();
                            List<Phone> phones = new List<Phone>();

                            foreach (string n in numbers)
                            {
                                int pNumber = (int)Convert.ToInt64(n);
                                Phone phone = new Phone { Number = pNumber.ToString(), Person = person };
                                phones.Add(phone);
                            }

                            _context.BulkInsert(phones);
                            _context.SaveChanges();

                            transaction.Commit();
                        }
                        catch
                        {
                            try
                            {
                                transaction.RollbackToSavepoint("NewPerson");
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
                        ModelState.AddModelError("", "This Person Already Create!");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Person Already Create!");
                }
            }
            return View(person);
        }

        // GET: People/Edit/5
        [CustomAuthorization]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Peoples.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            ViewBag.phone = _context.Phones.Where(e => e.Person.Cpf == id).ToList();

            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorization]
        public IActionResult Edit(string id, [Bind("Cpf,Name,Email,BirthDate,IsLibrarian,Password,CEP,State,City,Address,HouseNumber")] Person person)
        {
            if (id != person.Cpf)
            {
                return NotFound();
            }

            ViewBag.phone = _context.Phones.Where(e => e.Person.Cpf == id).ToList();

            if (ModelState.IsValid)
            {
                if (BeUniqueEmail(person.Email))
                {
                    using var transaction = _context.Database.BeginTransaction();

                    try
                    {
                        transaction.CreateSavepoint("BeforeEditPerson");

                        List<string> numbers = Request.Form["Number"].ToList();
                        List<Phone> phonesFromView = new List<Phone>();
                        List<Phone> phonesFromDB = _context.Phones.Where(e => e.Person.Cpf == id).ToList();
                        List<Phone> phonesFromDBCheck = new List<Phone>();

                        phonesFromDBCheck.AddRange(phonesFromDB);

                        foreach (string n in numbers)
                        {
                            int pNumber = (int)Convert.ToInt64(n);
                            Phone phone = new Phone { Number = pNumber.ToString(), Person = person };
                            phonesFromView.Add(phone);
                        }

                        foreach (Phone p in phonesFromDBCheck)
                        {
                            bool exist = phonesFromView.Any(a => a.Number == p.Number);

                            if (exist)
                            {
                                phonesFromView.Remove(p);
                            }
                        }

                        foreach (Phone p in phonesFromView)
                        {
                            bool exist = phonesFromDBCheck.Any(a => a.Number == p.Number);
                            if (exist)
                            {
                                phonesFromDB.Remove(p);
                            }
                        }

                        _context.BulkDelete(phonesFromDB);
                        _context.BulkInsert(phonesFromView);
                        _context.SaveChanges();

                        transaction.Commit();

                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PersonExists(person.Cpf))
                        {
                            return NotFound();
                        }
                        else
                        {
                            try
                            {
                                transaction.RollbackToSavepoint("BeforeEditPerson");
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
                    ModelState.AddModelError("", "You Can't Modify this!");
                }
            }
            return View(person);
        }

        // GET: People/Delete/5
        [CustomAuthorization]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Peoples
                .FirstOrDefaultAsync(m => m.Cpf == id);

            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorization]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var person = await _context.Peoples.FindAsync(id);
            List<Phone> phones = _context.Phones.Where(a => a.Person == person).ToList();

            using var transaction = _context.Database.BeginTransaction();
            transaction.CreateSavepoint("BeforeDeletePerson");

            try
            {
                _context.Peoples.Remove(person);
                await _context.SaveChangesAsync();

                _context.BulkDelete(phones);

                transaction.Commit();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                try
                {
                    transaction.RollbackToSavepoint("BeforeDeletePerson");
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

        private bool PersonExists(string id)
        {
            return _context.Peoples.Any(e => e.Cpf == id);
        }

        private bool BeUniqueEmail(string email)
        {
            return _context.Peoples.FirstOrDefault(x => x.Email == email) == null;
        }

        private bool BeUniquePhone(Phone phone)
        {
            return _context.Peoples.FirstOrDefault(x => x.Phones == phone) == null;
        }

        private string EncryptPassword(string cpf, string password)
        {
            byte[] saltyPassword = (Encoding.UTF8.GetBytes(cpf + password)); ;
            byte[] hashPassword;

            string result = string.Empty;

            SHA512 sha512 = new SHA512Managed();
            hashPassword = sha512.ComputeHash(saltyPassword);

            foreach (byte hash in hashPassword)
            {
                result += hash.ToString();
            }

            return result;
        }
    }
}
