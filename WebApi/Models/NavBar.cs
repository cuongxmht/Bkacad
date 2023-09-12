using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    [Table("NavBar")]
    public class NavBar{
        [Key]
        public string Id { get; set;}
        public string Title { get; set;}
        public ICollection<SubNav> SubNavs { get; set;}=new List<SubNav>();
    }
}