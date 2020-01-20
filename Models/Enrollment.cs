using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        [Display(Name = "Course")]
        public int CourseID { get; set; }
        [Display(Name = "Student")]
        public int StudentID { get; set; }
        [DisplayFormat(NullDisplayText = "No Grade Yet")]
        public Grade? Grade { get; set; }
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }


    }
    public enum Grade
    {
        A, B, C, D, F
    }
}