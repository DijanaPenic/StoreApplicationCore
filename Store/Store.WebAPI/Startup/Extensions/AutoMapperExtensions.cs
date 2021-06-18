using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;

using Store.DAL.Context;
using Store.Repository.Mapper;
using Store.WebAPI.Mapper.Profiles;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class AutoMapperExtensions
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper((serviceProvider, automapper) =>
            {
                automapper.AddExpressionMapping();

                // Configure Profiles
                automapper.AddProfile<AutoMapperWebApiProfile>();
                automapper.AddProfile<AutoMapperRepositoryProfile>();

            }, typeof(ApplicationDbContext).Assembly);
        }
    }
}
