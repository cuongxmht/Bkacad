using VicemMvcIdentity.Models.Entities;

namespace VicemMvcIdentity.Models.ViewModels
{
    public class UserWithRoleVM
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}