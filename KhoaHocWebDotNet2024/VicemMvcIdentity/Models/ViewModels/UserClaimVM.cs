using System.Security.Claims;

namespace VicemMvcIdentity.Models.ViewModels
{
    public class UserClaimVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<Claim> UserClaims { get; set; }
        public List<string> SelectedClaims { get; set; }
        public UserClaimVM()
        {
            UserId="";UserName="";UserClaims=new List<Claim>();
        }
        public UserClaimVM(string userId, string userName, List<Claim> userClaims)
        {
            UserId = userId;
            UserName = userName;
            UserClaims = userClaims;
        }
        
    }
    public class UserClaim{
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}