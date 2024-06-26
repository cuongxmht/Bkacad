namespace VicemMVCIdentity.Models.ViewModels
{
    public class RoleClaimVM
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleClaim> Claims { get; set; }
    }
    public class RoleClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}