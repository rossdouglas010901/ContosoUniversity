using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models
{
    public abstract class Person
    {
        // THE ID property will becoe the PK Column of the databse tabel
        //by default, the entitiy Framework (EF) interprets a property names "ID" or
        //"ClassnameID" as the PRimary key (PK)
        public int ID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Oops! First Name cannot be longer than 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(65, ErrorMessage = "Uh oh! Last Name cannot be longer than 65 characters")]
        [Display(Name = "First Name")]
        public string LastName { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "Yikes, Email cannot be longer than 255 characters")]
        public string Email { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Whoops! Address cannot be longer than 150 characters")]
        public string Address { get; set; }
        [Required]
        [StringLength(80, ErrorMessage = "Whoopsie, Your City Name cannot be longer than 80 characters")]
        public string City { get; set; }
        [Required]
        [StringLength(2)]
        [Column(TypeName="nchar(2)")]
        public string Province { get; set; }
        [Required]
        [StringLength(7)]
        [Column(TypeName = "nchar(7)")]
        [Display(Name = "First Name")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;

            }
        }
        public string FullNameAlt
        {
            get
            {
                return LastName + " " + FirstName;

            }
        }
        public string IDFullName
        {
            get
            {
                return "(" + ID + ") " + FullName;
            }
        }
        public string FullAddress
        {
            get
            {
                return Address + " " + City + ", " + Province + " " + PostalCode;
            }
        }

    }
}
