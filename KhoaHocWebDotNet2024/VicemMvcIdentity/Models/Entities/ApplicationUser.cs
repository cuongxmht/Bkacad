using Microsoft.AspNetCore.Identity;

namespace VicemMvcIdentity.Models.Entities
{
    public class ApplicationUser: IdentityUser
    {
        [PersonalData]
        public string? FullName { get; set; }
    }
}