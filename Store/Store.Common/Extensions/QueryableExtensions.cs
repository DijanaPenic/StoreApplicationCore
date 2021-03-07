using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Store.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string orderBy, bool isDescendingSortOrder)
        {
            Type type = typeof(TSource);
            string method = isDescendingSortOrder ? "OrderByDescending" : "OrderBy";

            PropertyInfo property = type.GetProperty(orderBy);
            ParameterExpression parameter = Expression.Parameter(type, "p");
            MemberExpression propertyAccess = Expression.MakeMemberAccess(parameter, property);
            LambdaExpression orderByExpression = Expression.Lambda(propertyAccess, parameter);

            MethodCallExpression resultExpression = Expression.Call
            (
                typeof(Queryable), 
                method, 
                new Type[] { type, orderByExpression.ReturnType }, 
                source.Expression, 
                Expression.Quote(orderByExpression)
            );

            return source.Provider.CreateQuery<TSource>(resultExpression);
        }

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
    }
}