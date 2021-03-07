using System;
using System.Linq;
using AutoMapper;

namespace Store.Common.Helpers
{
    public static class ModelMapperHelper
    {
        public static string[] GetPropertyMappings<TDestination, TSource>(IMapper mapper, params string[] properties)
        {
            if (properties != null && properties.Length > 0)
            {
                TypeMap mapping = GetTypeMap<TDestination, TSource>(mapper);

                for (int i = 0; i < properties.Length; i++)
                {
                    properties[i] = GetSourceProperty(mapping, properties[i]);
                }
            }

            properties = properties.Where(p => string.IsNullOrEmpty(p)).ToArray();

            return properties;
        }

        public static string GetPropertyMapping<TDestination, TSource>(IMapper mapper, string property)
        {
            TypeMap mapping = GetTypeMap<TDestination, TSource>(mapper);      

            return GetSourceProperty(mapping, property);
        }

        private static TypeMap GetTypeMap<TDestination, TSource>(IMapper mapper)
        {
            Type destinationType = typeof(TDestination);
            Type sourceType = typeof(TSource);

            TypeMap mapping = mapper.ConfigurationProvider.GetAllTypeMaps().FirstOrDefault(p => p.DestinationType.Equals(destinationType) && p.SourceType.Equals(sourceType));

            return mapping;
        }

        // Property mapping is not supported in the following use cases:
        // * If we have multiple source members. For example: ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
        // * Sorting by child entity. For example: sort books by bookstore name
        private static string GetSourceProperty(TypeMap mapping, string property)
        {
            PropertyMap dstProp = mapping.PropertyMaps.FirstOrDefault(p => p.DestinationName.Equals(property));

            if(dstProp != null)
            { 
                if (dstProp.SourceMembers.Count > 0)
                    return dstProp.SourceMembers.FirstOrDefault().Name;

                return dstProp.SourceMember.Name;
            }

            // Couldn't map to destination property
            return default;
        }
    }
}