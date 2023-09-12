using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models{

[Table("Person")]
public class Person
{
    [Key]
    public string PersonId { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public DateTime Birthdate { get; set; }
}
}