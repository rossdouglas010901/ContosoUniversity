using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name="Course Number")]
        public int CourseID { get; set; }
        [Required]
        [StringLength(50,MinimumLength = 3)]
        public string Title { get; set; }
        [Range(3,6)]
        public int Credits { get; set; }
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }
        public string CourseIDTitle
        {
            get
            {
                return CourseID + ": " + Title;
            }
        }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual Department Department { get; set; }
    }
}