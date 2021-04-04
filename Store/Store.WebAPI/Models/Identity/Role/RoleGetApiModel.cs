namespace Store.WebAPI.Models.Identity
{
    public class RoleGetApiModel : ApiModelBase
    {
        public string Name { get; set; }

        public bool Stackable { get; set; }

        public PolicyGetApiModel[] Policies { get; set; }
    }
}