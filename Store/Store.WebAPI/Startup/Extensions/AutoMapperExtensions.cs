using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;

using Store.Repository.Mapper;
using Store.WebAPI.Mapper.Profiles;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class AutoMapperExtensions
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperWebApiProfile>();
                cfg.AddProfile<AutoMapperRepositoryProfile>();

                cfg.AddExpressionMapping();
            });

            IMapper mapper = mapperConfiguration.CreateMapper();

            services.AddSingleton(mapper);
        }
    }
}
