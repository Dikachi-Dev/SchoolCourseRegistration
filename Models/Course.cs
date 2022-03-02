using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolCourseRegistration.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string CourseHandler { get; set; }
        public int CreditUnit { get; set; }
        public int Semester { get; set; }
        public int Level { get; set; }
        public int NumberOfReg { get; set; }
    }
}
