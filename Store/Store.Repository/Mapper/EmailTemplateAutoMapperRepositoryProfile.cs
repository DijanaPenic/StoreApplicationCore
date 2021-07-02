using AutoMapper;

using Store.Entities;
using Store.Model.Common.Models;

namespace Store.Repository.Mapper
{
    public class EmailTemplateAutoMapperRepositoryProfile : Profile
    {
        public EmailTemplateAutoMapperRepositoryProfile()
        {
            CreateMap<IEmailTemplate, EmailTemplateEntity>()
                .ForMember(dst => dst.Client, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}