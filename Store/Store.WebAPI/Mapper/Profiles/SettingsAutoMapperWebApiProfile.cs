using AutoMapper;

using Store.Model.Common.Models;
using Store.WebAPI.Models.Settings;

namespace Store.WebAPI.Mapper.Profiles
{
    public class SettingsAutoMapperWebApiProfile : Profile
    {
        public SettingsAutoMapperWebApiProfile()
        {
            CreateMap<EmailTemplateGetApiModel, IEmailTemplate>().ReverseMap();
            CreateMap<CountryGetApiModel, ICountry>().ReverseMap();
        }
    }
}