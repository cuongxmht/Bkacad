using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models{
    [Table("SubNav")]
    public class SubNav{
        [Key]
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Title { get; set; }
        public NavBar NavBar { get; set; }
    }
}