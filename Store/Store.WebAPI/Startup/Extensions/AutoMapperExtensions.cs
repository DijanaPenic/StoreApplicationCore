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

                // Configure Profiles
                automapper.AddProfile<AutoMapperRepositoryProfile>();
                automapper.AddProfile<AutoMapperWebApiProfile>();

            }, typeof(AutoMapperRepositoryProfile).Assembly, typeof(AutoMapperWebApiProfile).Assembly);
        }
    }
}
