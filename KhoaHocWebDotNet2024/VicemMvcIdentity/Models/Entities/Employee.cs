using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VicemMvcIdentity.Models.Entities
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public string? Address { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string? NonSignName { get; set; }
        [Required]
        public string? Position { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }
    }
}