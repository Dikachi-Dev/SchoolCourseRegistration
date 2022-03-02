using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolCourseRegistration.Data;
using SchoolCourseRegistration.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace SchoolCourseRegistration.Controllers
{
    public class HomeController : Controller
    {


        //public HomeController(SchoolCourseRegistrationContext context)
        //{
        //    _context = context;
        //}
        private readonly ILogger<HomeController> _logger;
        private readonly SchoolCourseRegistrationContext _context;

        public HomeController(ILogger<HomeController> logger, SchoolCourseRegistrationContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index(User user)
        {
            var account = _context.User.SingleOrDefault(x => x.Email == user.Email);
            if (account == null)
            {
                return View();
            }



            if (BC.Verify(user.Password, user.PassHash))
            {


                if (account.Role == "Admin")
                {
                    return RedirectToAction(nameof(Index));
                }
                //var app = await _context.User.FirstOrDefaultAsync(m => m.UserName == user.UserName);
                //ViewBag.Transac = _context.Transaction.Where(x => x.To == account.AccountNo || x.From == account.UserName).ToList();
                //ViewBag.Interbank = _context.Interbank.Where(x => x.Me == account.UserName).ToList();
                ViewBag.User = account;
                ViewBag.Profile = _context.User.FirstOrDefault(x => x.Email == account.Email);
                return View("Home", account);
            }
            TempData["Message"] = "Wrong Credentials";
            return View();
            //return View();
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

