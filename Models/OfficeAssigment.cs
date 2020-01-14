using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class OfficeAssigment
    {
        [Key]
        public int InstrutorID { get; set; }
        [Display(Name = "Office Space Location")]
        [StringLength(60)]
        public string Location { get; set; }
        public virtual Instructor Instructor { get; set; }
    }
}