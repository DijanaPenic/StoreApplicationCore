using System.Text.Json;

using Store.Models;
using Store.Models.Identity;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Common.Infrastructure.Serialization;

namespace Store.Cache.Serialization
{
    public static class JsonSerializerInit
    {
        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonTypeMappingConverter<IBookstore, Bookstore>(),
                    new JsonTypeMappingConverter<IBook, Book>(),
                    new JsonTypeMappingConverter<IRole, Role>(),
                    new JsonTypeMappingConverter<ICountry, Country>()
                }
            };
        }
    }
}