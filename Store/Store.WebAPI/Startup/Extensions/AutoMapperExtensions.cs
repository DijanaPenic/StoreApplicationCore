using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;

using Store.Repository.Mapper;
using Store.WebAPI.Mapper.Profiles;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class AutoMapperExtensions
    {
        public static void AddAutoMapperServices(this IServiceCollection services)
        {
            services.AddAutoMapper((serviceProvider, automapper) =>
            {
                automapper.AddExpressionMapping();

                // Configure Repository Profiles
                automapper.AddProfile<BookAutoMapperRepositoryProfile>();
                automapper.AddProfile<BookstoreAutoMapperRepositoryProfile>();
                automapper.AddProfile<EmailTemplateAutoMapperRepositoryProfile>();
                automapper.AddProfile<IdentityAutoMapperRepositoryProfile>();
                
                // Configure Web API profiles
                automapper.AddProfile<BookAutoMapperWebApiProfile>();
                automapper.AddProfile<BookstoreAutoMapperWebApiProfile>();
                automapper.AddProfile<GlobalSearchAutoMapperWebApiProfile>();
                automapper.AddProfile<IdentityAutoMapperWebApiProfile>();
                automapper.AddProfile<PagingAutoMapperWebApiProfile>();
                automapper.AddProfile<SettingsAutoMapperWebApiProfile>();
                
            }, typeof(BookAutoMapperRepositoryProfile).Assembly, typeof(BookAutoMapperWebApiProfile).Assembly);
        }
    }
}
