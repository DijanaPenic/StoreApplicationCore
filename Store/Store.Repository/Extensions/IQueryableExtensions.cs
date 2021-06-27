using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using LinqKit;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using Store.DAL.Context;
using Store.Common.Parameters.Sorting;

namespace Store.Repository.Extensions
{
    public static class QueryableExtensions
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

        public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> source, IMapper mapper, string[] includeParameters)
        {
            includeParameters ??= Array.Empty<string>();

            return source.ProjectTo<TDestination>(mapper.ConfigurationProvider, null, includeParameters);
        }

        private static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderBy, bool isFirstParam)
        {
            return OrderBy(source, orderBy, false, isFirstParam);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, ISortingParameters sortingParameters)
        {
            if (sortingParameters?.Sorters == null) return source;

            int sortingCount = sortingParameters.Sorters.Count;
            if (sortingCount == 0) return source;

            IQueryable<T> query = source;
            for (int i = 0; i < sortingCount; i++)
            {
                ISortingPair item = sortingParameters.Sorters[i];

                query = item.Ascending ? query.OrderBy(item.OrderBy, i == 0) : query.OrderByDescending(item.OrderBy, i == 0);
            }

            return query;
        }

        private static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string orderBy, bool isFirstParam)
        {
            return OrderBy(source, orderBy, true, isFirstParam);
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, ApplicationDbContext dbContext, params object[] keyValues)
        {
            return source.FirstOrDefaultAsync(GetPrimaryKeyPredicate<TSource>(dbContext, keyValues));
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, ApplicationDbContext dbContext, params object[] keyValues)
        {
            return source.SingleOrDefaultAsync(GetPrimaryKeyPredicate<TSource>(dbContext, keyValues));
        }

        private static Expression<Func<TSource, bool>> GetPrimaryKeyPredicate<TSource>(ApplicationDbContext dbContext, params object[] keyValues)
        {
            string[] keyNames = dbContext.Model.FindEntityType(typeof(TSource)).FindPrimaryKey().Properties.Select(p => p.Name).ToArray();
            IEnumerable<(string Name, object Value)> keys = keyValues.Zip(keyNames, (v, n) => (Name: n, Value: v));

            ExpressionStarter<TSource> predicate = PredicateBuilder.New<TSource>();
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), "e");

            foreach ((string name, object value) in keys)
            {
                //create the lambda expression
                MemberExpression predicateLeft = Expression.PropertyOrField(parameter, name);
                ConstantExpression predicateRight = Expression.Constant(value);

                predicate = predicate.And(Expression.Lambda<Func<TSource, bool>>(Expression.Equal(predicateLeft, predicateRight), parameter));
            }

            return predicate;
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

                    propertyAccess = Expression.MakeMemberAccess(previousPropertyAccess ?? firstPropertyAccess, property);

                    propertyType = property.PropertyType;
                    previousPropertyAccess = propertyAccess;
                }

                propertyAccess ??= firstPropertyAccess;

                LambdaExpression orderByExp = Expression.Lambda(propertyAccess, parameter);
                MethodCallExpression resultExp = Expression.Call(typeof(Queryable), method, new Type[] { type, propertyType }, source.Expression, Expression.Quote(orderByExp));

                return source.Provider.CreateQuery<T>(resultExp);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid sort expression [{orderBy}].", ex);
            }
        }
    }
}