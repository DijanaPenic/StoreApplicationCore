using Store.Common.Enums;

namespace Store.WebAPI.Models.Settings
{
    public class EmailTemplateGetApiModel : ApiModelBase
    {
        public string Name { get; set; }

        public EmailTemplateType Type { get; set; }
    }
}