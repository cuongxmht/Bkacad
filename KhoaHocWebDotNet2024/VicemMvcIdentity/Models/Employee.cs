using System.ComponentModel.DataAnnotations;

namespace VicemMvcIdentity.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set;}
        public string? FullName { get; set;}
    }
}