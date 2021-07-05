using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using AutoMapper;

using Store.Common.Extensions;
using Store.Common.Parameters.Sorting;

namespace Store.Common.Helpers
{
    public static class ModelMapperHelper
    {
        private static readonly ConcurrentDictionary<string, string> SortPropertyMappings = new();
        private static readonly ConcurrentDictionary<string, string> PropertyMappings = new();

        public static string[] GetSortPropertyMappings<TDestination, TSource>(IMapper mapper, string sortExpression)
        {
            IList<string> result = new List<string>();
            if (string.IsNullOrWhiteSpace(sortExpression))
            {
                return result.ToArray();
            }

            Type dstType = typeof(TDestination);
            Type srcType = typeof(TSource);

            TypeMap mapping = GetTypeMap<TDestination, TSource>(mapper);
            string[] properties = sortExpression.Split(',');

            foreach (string property in properties)
            {
                if (!property.Contains(SortingParameters.Separator)) continue;
                
                string[] sortFragments = property.Split(SortingParameters.Separator);
                if (sortFragments.Length != 2) continue;
                    
                string srcProperty = SortPropertyMappings.GetOrAdd
                (
                    $"{srcType.FullName}{dstType.FullName}{sortFragments[0].ToPascalCase()}",
                    GetSourceProperty(mapper, mapping, property: sortFragments[0])
                );

                if (!string.IsNullOrWhiteSpace(srcProperty))
                {
                    result.Add($"{sortFragments[0]}{SortingParameters.Separator}{sortFragments[1]}");
                }
            }

            return result.ToArray();
        }

        public static ISortingParameters GetSortPropertyMappings<TDestination, TSource>(IMapper mapper, ISortingParameters sorting)
        {
            if (sorting.Sorters == null) return sorting;
            
            foreach (ISortingPair sortPair in sorting.Sorters)
            {
                TypeMap mapping = GetTypeMap<TDestination, TSource>(mapper);
                sortPair.OrderBy = GetSourceProperty(mapper, mapping, sortPair.OrderBy);
            }

            return sorting;
        }

        public static string[] GetPropertyMappings<TDestination, TSource>(IMapper mapper, string[] properties)
        {
            IList<string> result = new List<string>();

            if (properties is {Length: 0}) return default;
            
            Type dstType = typeof(TDestination);
            Type srcType = typeof(TSource);

            TypeMap mapping = GetTypeMap<TDestination, TSource>(mapper);

            foreach (string property in properties)
            {
                string srcProperty = PropertyMappings.GetOrAdd
                (
                    $"{srcType.FullName}{dstType.FullName}{property.ToPascalCase()}", 
                    GetSourceProperty(mapper, mapping, property)
                );

                if(!string.IsNullOrWhiteSpace(srcProperty))
                    result.Add(srcProperty);
            }

            return result.ToArray();
        }

        public static string[] GetPropertyMappings<TDestination, TSource>(IMapper mapper, string properties)
        {
            return string.IsNullOrWhiteSpace(properties) ? Array.Empty<string>() : GetPropertyMappings<TDestination, TSource>(mapper, properties.Split(','));
        }

        public static string GetPropertyMapping<TDestination, TSource>(IMapper mapper, string property)
        {
            TypeMap mapping = GetTypeMap<TDestination, TSource>(mapper);      

            return GetSourceProperty(mapper, mapping, property);
        }

        private static TypeMap GetTypeMap<TDestination, TSource>(IMapper mapper)
        {
            Type destinationType = typeof(TDestination);
            Type sourceType = typeof(TSource);

            TypeMap mapping = mapper.ConfigurationProvider.GetAllTypeMaps().FirstOrDefault(p => p.DestinationType == destinationType && p.SourceType == sourceType);

            if (mapping == null)
            {
                if (sourceType != null)
                {
                    mapper.Map(sourceType, destinationType);
                    mapping = mapper.ConfigurationProvider.GetAllTypeMaps().FirstOrDefault(p => p.DestinationType == destinationType);
                }
                if (mapping == null)
                {
                    throw new Exception("Mapping cannot be created!");
                }
            }

            return mapping;
        }

        // Property mapping is not supported in the following use cases:
        // * If we have multiple source members. For example: ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
        private static string GetSourceProperty(IMapper mapper, TypeMap mapping, string property)
        {
            List<string> srcProperties = new List<string>();
            List<string> dstProperties = property.Split('.', StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (string dstProperty in dstProperties)
            {
                if (mapping == null)
                {
                    throw new ArgumentOutOfRangeException(property);
                }

                PropertyMap propertyMapping = mapping.PropertyMaps.FirstOrDefault(p => p.DestinationName.Equals(dstProperty.ToPascalCase()));
                if (propertyMapping == null)
                {
                    return null;
                }

                if (propertyMapping.SourceMember != null)
                {
                    srcProperties.Add(propertyMapping.SourceMember.Name);
                }
                else if (propertyMapping.CustomMapExpression?.Body != null)
                {
                    srcProperties.AddRange(propertyMapping.CustomMapExpression.Body.ToString().Split('.').Skip(1).ToList());
                }

                // TODO - check array types
                mapping = mapper.ConfigurationProvider.GetAllTypeMaps().FirstOrDefault(p => p.DestinationType == ((PropertyInfo)propertyMapping.DestinationMember).PropertyType);
            }

            return srcProperties.Count > 0 ? string.Join('.', srcProperties) : property;
        }
    }
}