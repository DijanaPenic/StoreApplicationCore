namespace Store.WebAPI.Models.Identity
{
    public class UserClaimGetApiModel : ApiModelBase
    {
        public string ClaimType { get; set; }
         
        public string ClaimValue { get; set; }
    }
}