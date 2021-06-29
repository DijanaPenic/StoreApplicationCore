using System.Text.Json;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Infrastructure.Serialization;

namespace Store.Service.Serialization
{
    internal static class JsonSerializerInit
    {
        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new()
            {
                Converters =
                {
                    new JsonTypeMappingConverter<ICountry, Country>()
                }
            };
        }
    }
}