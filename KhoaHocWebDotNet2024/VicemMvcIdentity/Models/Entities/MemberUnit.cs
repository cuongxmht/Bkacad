using System.ComponentModel.DataAnnotations;

namespace VicemMvcIdentity.Models.Entities
{
    public class MemberUnit
    {
        [Key]
        public int MemberUnitId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string WebsiteUrl { get; set; }
        public MemberUnit() { }
        public MemberUnit(int id, string name, string address, string phoneNumber, string websiteUrl)
        {
            MemberUnitId = id;
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            WebsiteUrl = websiteUrl;
        }
    }
}


