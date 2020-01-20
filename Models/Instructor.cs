using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Instructor : Person
    {
        [DataType(DataType.Date)]
        [Display(Name = "Enrollment Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy - MM - dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }
        public virtual ICollection<CourseAssignment> Courses { get; set; }
        public virtual OfficeAssignment OfficeAssignment { get; set;}
    }
}