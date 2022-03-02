using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCourseRegistration.Data;
using SchoolCourseRegistration.Models;

namespace SchoolCourseRegistration.Controllers
{
    public class CourseRegsController : Controller
    {
        private readonly SchoolCourseRegistrationContext _context;

        public CourseRegsController(SchoolCourseRegistrationContext context)
        {
            _context = context;
        }

        // GET: CourseRegs
        public async Task<IActionResult> Index()
        {
            return View(await _context.CourseReg.ToListAsync());
        }

        // GET: CourseRegs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseReg = await _context.CourseReg
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseReg == null)
            {
                return NotFound();
            }

            return View(courseReg);
        }

        // GET: CourseRegs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CourseRegs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseCode,CourseTitle,CourseHandler,CreditUnit,MatNo,Semester,LName,FName,Level,Status")] CourseReg courseReg, Course course)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(courseReg);
                courseReg.Status = "Pending";
                
               

                //course.NumberOfReg = _context.Course.AsEnumerable().Where(x => x.CourseTitle == courseReg.CourseTitle).ToList().Count;
                var cours = _context.Course.FirstOrDefault(x => x.CourseCode == courseReg.CourseCode);
                int plus = 1;
                cours.NumberOfReg += plus;
               
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(courseReg);
        }

        // GET: CourseRegs/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,CourseCode,CourseTitle,CourseHandler,CreditUnit,MatNo,Semester,LName,FName,Level,Status")] CourseReg courseReg, Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseReg);
                courseReg.Status = "Pending";



               
                var cours = _context.Course.FirstOrDefault(x => x.CourseCode == courseReg.CourseCode);
                int plus = 1;
                cours.NumberOfReg += plus;

                await _context.SaveChangesAsync();
                TempData["Message"] = "Registration Successful";
                return RedirectToAction("Home", "Users"); // redirect to Home page
            }
            return View(courseReg);
        }

        public IActionResult Done()
        {
            return View();
        }

        // GET: CourseRegs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseReg = await _context.CourseReg.FindAsync(id);
            if (courseReg == null)
            {
                return NotFound();
            }
            return View(courseReg);
        }

        // POST: CourseRegs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseCode,CourseTitle,CourseHandler,CreditUnit,MatNo,Semester,LName,FName,Level,Status")] CourseReg courseReg)
        {
            if (id != courseReg.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseReg);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseRegExists(courseReg.Id))
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
            return View(courseReg);
        }

        // GET: CourseRegs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseReg = await _context.CourseReg
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseReg == null)
            {
                return NotFound();
            }

            return View(courseReg);
        }

        // POST: CourseRegs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseReg = await _context.CourseReg.FindAsync(id);
            _context.CourseReg.Remove(courseReg);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseRegExists(int id)
        {
            return _context.CourseReg.Any(e => e.Id == id);
        }
    }
}
