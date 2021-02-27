namespace Store.WebAPI.Models.Identity
{
    public class PolicyGetApiModel
    {
        public string Section { get; set; }

        public AccessActionGetApiModel[] AccessActions { get; set; }
    }
}