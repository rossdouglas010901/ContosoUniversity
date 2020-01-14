using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        [Required]
        [StringLength(60,MinimumLength = 3)]
        public string Name { get; set; }
        [DataType(DataType.Currency)]
        public decimal Budget { get; set; }
        [DataType(DataType.Date)]
        [Display(Name="Date Created")]
        [DisplayFormat(DataFormatString = "{0:yyyy - MM - dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Administrator")]
        public int? InstructorID { get; set; }
        public virtual Instructor Administrator { get; set; }
        public virtual ICollection<Course> COurses { get; set; }

    }
}