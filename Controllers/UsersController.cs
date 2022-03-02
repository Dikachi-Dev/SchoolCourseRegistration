using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RestSharp;
using SchoolCourseRegistration.Data;
using SchoolCourseRegistration.Models;
using BC = BCrypt.Net.BCrypt;


namespace SchoolCourseRegistration.Controllers
{

    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;

        private readonly SchoolCourseRegistrationContext _context;

        public UsersController(ILogger<UsersController> logger, SchoolCourseRegistrationContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Password,PassHash,Role,Level,MatNo,LName,FName")] User user)
        {
            if (ModelState.IsValid)
            {
                if (_context.User.Any(x => x.Email == user.Email))
                {
                    return View();
                }
                var firstAccount = _context.User.Count() == 0;
                user.Role = firstAccount ? user.Role = "Admin" : user.Role = "Student";

                user.PassHash = BC.HashPassword(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["Message"] = "User Registration Successful";
                return RedirectToAction("Home", "Users");
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password,PassHash,Role,Level,MatNo,LName,FName")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }


        public IActionResult Home([Bind("Id, Email,Role, Password")] User user)
        {
            if (Request.Cookies["logininfo"] == null)
            {
                var account = _context.User.SingleOrDefault(x => x.Email == user.Email);
                if (account == null)
                {
                    return View();
                }
                if (BC.Verify(user.Password, account.PassHash))
                {
                    string logininfo = JsonSerializer.Serialize(account);
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddMinutes(10);
                    option.SameSite = SameSiteMode.Unspecified;
                    option.Secure = true;
                    Response.Cookies.Append("logininfo", logininfo, option);
                    if (account.Role == "Admin")
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.User = account;
                        return View("Student", account);
                    }
                }
                TempData["Message"] = "Wrong Credentials";
                return View();
            }
            else
            {
                string lop = Request.Cookies["logininfo"];
                var act = JsonSerializer.Deserialize<User>(lop);
                if (user.Email == act.Email && user.Password == act.Password)
                {
                    if (act.Role == "Admin")
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ViewBag.User = act;
                    return View("Student", act);
                }
                else
                {
                    TempData["Message"] = "Wrong Credentials";
                    return View();
                }

            }

        }
        public IActionResult Search(string searchstring)
        {
            if (Request.Cookies["search"] != null && Request.Cookies["search"] == searchstring)
            {
                var look = _context.CourseReg.Where(x => x.MatNo == searchstring).ToList();
                if (look != null)
                {
                    return View("Registered", look);
                }
                else
                {
                    TempData["Message"] = "No Course Registered";
                    return View("CourseReg");
                }
                
            }
            else
            {
                if (!String.IsNullOrEmpty(searchstring))
                {
                    string search = searchstring;
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddMinutes(10);
                    option.SameSite = SameSiteMode.Unspecified;
                    option.Secure = true;
                    Response.Cookies.Append("search", search, option);
                    var look = _context.CourseReg.Where(x => x.MatNo == Request.Cookies["search"]).ToList();
                    return View("Registered", look);
                }
                return View();
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

