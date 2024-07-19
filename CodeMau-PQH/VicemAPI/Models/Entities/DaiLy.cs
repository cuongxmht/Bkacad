using System.ComponentModel.DataAnnotations;

namespace VicemAPI.Models.Entities
{
    public class DaiLy{
        [Key]
        public int DaiLyId { get; set;}
        public string TenDaiLy { get;set;}=default!;
        public string DiaChi { get; set; }=default!;
    }
}