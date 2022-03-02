using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolCourseRegistration.Models
{
    public class CourseReg
    {
        public int Id { get; set; }
        public string MatNo { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string CourseHandler { get; set; }
        public int CreditUnit { get; set; }
         public string FName { get; set; }
        public string LName { get; set; }
        public string Semester { get; set; }
        public string Level { get; set; }
        public string  Status { get; set; }
    }
}
