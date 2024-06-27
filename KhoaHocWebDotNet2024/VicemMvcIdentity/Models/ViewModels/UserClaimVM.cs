using System.Security.Claims;

namespace VicemMvcIdentity.Models.ViewModels
{
    public class UserClaimVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<Claim> UserClaims { get; set; }=new List<Claim>();
        public List<UserClaim> AllUserClaims { get; set; }=new List<UserClaim>();
        public UserClaimVM()
        {
            UserId="";UserName="";
        }
        public UserClaimVM(string userId, string userName, List<Claim> userClaims)
        {
            UserId = userId;
            UserName = userName;
            UserClaims = userClaims;
        }
        public UserClaimVM(string userId, string userName, List<Claim> userClaims, List<UserClaim> allUserClaims )
        {
            UserId = userId;
            UserName = userName;
            UserClaims = userClaims;
            AllUserClaims=allUserClaims;
        }
    }
    public class UserClaim{
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}