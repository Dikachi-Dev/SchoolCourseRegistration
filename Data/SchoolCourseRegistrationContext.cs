using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolCourseRegistration.Models;

namespace SchoolCourseRegistration.Data
{
    public class SchoolCourseRegistrationContext : DbContext
    {
        public SchoolCourseRegistrationContext (DbContextOptions<SchoolCourseRegistrationContext> options)
            : base(options)
        {
        }


        public DbSet<SchoolCourseRegistration.Models.Course> Course { get; set; }

        public DbSet<SchoolCourseRegistration.Models.CourseReg> CourseReg { get; set; }

        public DbSet<SchoolCourseRegistration.Models.User> User { get; set; }
    }
}
