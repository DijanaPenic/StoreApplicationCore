using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using Store.Common.Parameters.Sorting;

namespace Store.Common.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TSource> Include<TSource>(this IQueryable<TSource> source, string[] includeParameters) where TSource : class
        {
            if (includeParameters == null || includeParameters.Length == 0) return source;

            IQueryable<TSource> query = source;
            includeParameters.ToList().ForEach(p =>
            {
                if (!string.IsNullOrWhiteSpace(p))
                    query = query.Include(p);
            });

            return query;
        }

        public static IQueryable<TSource> Filter<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> filterExpression)
        {
            return filterExpression != null ? source.Where(filterExpression) : source;
        }

        public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> source, IMapper mapper, params string[] includeParameters)
        {
            includeParameters ??= Array.Empty<string>();

            return source.ProjectTo<TDestination>(mapper.ConfigurationProvider, null, includeParameters);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderBy, bool isFirstParam)
        {
            return OrderBy(source, orderBy, false, isFirstParam);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, ISortingParameters sortingParameters)
        {
            if (sortingParameters == null) return source;

            int sortingCount = sortingParameters.Sorters.Count;
            if (sortingCount == 0) return source;

            IQueryable<T> query = source;
            for (int i = 0; i < sortingCount; i++)
            {
                ISortingPair item = sortingParameters.Sorters[i];

                if (item.Ascending)
                    query = query.OrderBy(item.OrderBy, i == 0);
                else
                    query = query.OrderByDescending(item.OrderBy, i == 0);
            }

            return query;
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string orderBy, bool isFirstParam)
        {
            return OrderBy(source, orderBy, true, isFirstParam);
        }

        private static IQueryable<T> OrderBy<T>(IQueryable<T> source, string orderBy, bool descending, bool isFirstParam)
        {
            try
            {
                Type type = typeof(T);
                string method = isFirstParam ? (descending ? "OrderByDescending" : "OrderBy") : (descending ? "ThenByDescending" : "ThenBy");
                string propNameExpression = char.ToUpper(orderBy[0]) + orderBy[1..];

                string[] props = propNameExpression.Split('.');
                PropertyInfo firstProperty = type.GetProperty(props.First());
                ParameterExpression parameter = Expression.Parameter(type, "p");
                MemberExpression firstPropertyAccess = Expression.MakeMemberAccess(parameter, firstProperty);

                MemberExpression propertyAccess = null;
                Type propertyType = firstProperty.PropertyType;
                MemberExpression previousPropertyAccess = null;

                for (int i = 1; i < props.Length; i++)
                {
                    PropertyInfo property = propertyType.GetProperty(props[i]);

                    if (previousPropertyAccess == null)
                        propertyAccess = Expression.MakeMemberAccess(firstPropertyAccess, property);
                    else
                        propertyAccess = Expression.MakeMemberAccess(previousPropertyAccess, property);

                    propertyType = property.PropertyType;
                    previousPropertyAccess = propertyAccess;
                }

                if (propertyAccess == null)
                {
                    propertyAccess = firstPropertyAccess;
                }

                LambdaExpression orderByExp = Expression.Lambda(propertyAccess, parameter);
                MethodCallExpression resultExp = Expression.Call(typeof(Queryable), method, new Type[] { type, propertyType }, source.Expression, Expression.Quote(orderByExp));

                return source.Provider.CreateQuery<T>(resultExp);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Invalid sort expression [{0}].", orderBy), ex);
            }
        }
    }
}