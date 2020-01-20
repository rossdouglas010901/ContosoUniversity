using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class OfficeAssignment
    {
        [Key]
        public int InstructorID { get; set; }
        [Display(Name = "Office Space Location")]
        [StringLength(60)]
        public string Location { get; set; }
        public virtual Instructor Instructor { get; set; }
    }
}