using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Extensions;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Repository.Extensions;

namespace Store.Repository.Core
{
    internal abstract partial class GenericRepository
    {
        protected async Task<IEnumerable<TDomain>> GetAsync<TDomain, TEntity>
        (
            ISortingParameters sorting,
            IOptionsParameters options
        ) where TEntity : class
        {
            IEnumerable<TEntity> entities = await DbContext.Set<TEntity>()
                                                           .Include(OptionsMap<TDomain, TEntity>(options))
                                                           .OrderBy(SortingMap<TDomain, TEntity>(sorting))
                                                           .ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(entities);
        }

        protected async Task<IEnumerable<TDomain>> GetUsingProjectionAsync<TDomain, TEntity, TDTO>(IOptionsParameters options) where TEntity : class
        {
            IList<TDTO> destItems = await DbContext.Set<TEntity>().ProjectTo<TEntity, TDTO>(Mapper, OptionsMap<TDomain, TEntity>(options)).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(destItems);
        }

        protected async Task<IPagedList<TDomain>> FindAsync<TDomain, TEntity>
        (
            Expression<Func<TDomain, bool>> filterExpression, 
            IPagingParameters paging, 
            ISortingParameters sorting, 
            IOptionsParameters options
        ) where TEntity : class
        {
            IPagedList<TEntity> entityPagedList = await Find<TDomain, TEntity>(filterExpression, sorting, options).ToPagedListAsync(paging.PageNumber, paging.PageSize);

            return entityPagedList.ToPagedList<TEntity, TDomain>(Mapper);
        }

        protected async Task<IPagedList<TDomain>> FindWithProjectionAsync<TDomain, TEntity, TDTO>
        (
            Expression<Func<TDomain, bool>> filterExpression, 
            IPagingParameters paging, 
            ISortingParameters sorting, 
            IOptionsParameters options
        ) where TEntity : class
        {
            IPagedList<TDTO> destPagedList = await FindUsingProjection<TDomain, TEntity, TDTO>(filterExpression, sorting, options).ToPagedListAsync(paging.PageNumber, paging.PageSize);

            return destPagedList.ToPagedList<TDTO, TDomain>(Mapper);
        }

        protected async Task<IEnumerable<TDomain>> FindAsync<TDomain, TEntity>
        (
            Expression<Func<TDomain, bool>> filterExpression, 
            ISortingParameters sorting, 
            IOptionsParameters options
        ) where TEntity : class
        {
            IEnumerable<TEntity> entities = await Find<TDomain, TEntity>(filterExpression, sorting, options).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(entities);
        }

        protected async Task<IEnumerable<TDomain>> FindUsingProjectionAsync<TDomain, TEntity, TDTO>
        (
            Expression<Func<TDomain, bool>> filterExpression, 
            ISortingParameters sorting, 
            IOptionsParameters options
        ) where TEntity : class
        {
            IList<TDTO> destItems = await FindUsingProjection<TDomain, TEntity, TDTO>(filterExpression, sorting, options).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(destItems);
        }

        protected async Task<TDomain> FindByKeyAsync<TDomain, TEntity>
        (
            IOptionsParameters options,
            params object[] keyValues
        ) where TEntity : class
        {
            TEntity entity = await DbContext.Set<TEntity>().Include(OptionsMap<TDomain, TEntity>(options)).FirstOrDefaultAsync(DbContext, keyValues); 

            return Mapper.Map<TDomain>(entity);
        }

        protected async Task<TDomain> FindByKeyUsingProjectionAsync<TDomain, TEntity, TDTO>
        ( 
            IOptionsParameters options,
            params object[] keyValues
        ) where TEntity : class
        {
            TDTO destItem = await DbContext.Set<TEntity>()
                                           .ProjectTo<TEntity, TDTO>(Mapper, OptionsMap<TDomain, TEntity>(options))
                                           .FirstOrDefaultAsync(DbContext, keyValues); 

            return Mapper.Map<TDomain>(destItem);
        }

        protected Task<ResponseStatus> AddAsync<TDomain, TEntity>(TDomain model) where TEntity : class
        {
            if (model == null)
                Task.FromResult(ResponseStatus.Error);

            TEntity entity = Mapper.Map<TEntity>(model);

            DbContext.Set<TEntity>().Add(entity);

            return Task.FromResult(ResponseStatus.Success);
        }

        protected async Task<ResponseStatus> DeleteByKeyAsync<TDomain, TEntity>(params object[] keyValues) where TEntity : class
        {
            TEntity entity = await DbContext.Set<TEntity>().FindAsync(keyValues);

            if (entity == null)
                return ResponseStatus.NotFound;

            DbContext.Set<TEntity>().Remove(entity);

            return ResponseStatus.Success;
        }

        protected async Task<ResponseStatus> UpdateAsync<TDomain, TEntity>
        (
            TDomain model,
            params object[] keyValues
        ) where TEntity : class
        {
            if (model == null)
                return ResponseStatus.Error;

            // TEntity entity = Mapper.Map<TEntity>(model); -> can't use this for model updates
            // Need to search context or the store because AutoMapper creates a new instance of the object during domain->entity mapping and sometimes
            // the newly created instance cannot be attached to the context (if we're already tracking the original instance)
            TEntity entity = await DbContext.Set<TEntity>().FindAsync(keyValues);

            if (entity == null)
                return ResponseStatus.NotFound;

            // This works only for scalar property updates. The navigation property mapping is out of the scope for AutoMapper.
            // Also, the author is highly suggesting against the domain->entity mappings.
            Mapper.Map(model, entity);
            
            DbContext.Entry(entity).State = EntityState.Modified;

            return ResponseStatus.Success;
        }

        protected ISortingParameters SortingMap<TDomain, TEntity>(ISortingParameters sorting) => ModelMapperHelper.GetSortPropertyMappings<TDomain, TEntity>(Mapper, sorting);

        protected string[] OptionsMap<TDomain, TEntity>(IOptionsParameters options) => ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);

        private Expression<Func<TEntity, bool>> FilterMap<TDomain, TEntity>(Expression<Func<TDomain, bool>> filterExpression) => Mapper.Map<Expression<Func<TEntity, bool>>>(filterExpression);

        private IQueryable<TEntity> Find<TDomain, TEntity>
        (
            Expression<Func<TDomain, bool>> filterExpression, 
            ISortingParameters sorting, 
            IOptionsParameters options
        ) where TEntity : class
        {
            return DbContext.Set<TEntity>()
                            .Filter(FilterMap<TDomain, TEntity>(filterExpression))
                            .Include(OptionsMap<TDomain, TEntity>(options))
                            .OrderBy(SortingMap<TDomain, TEntity>(sorting));
        }

        private IQueryable<TDestination> FindUsingProjection<TDomain, TEntity, TDestination>
        (
            Expression<Func<TDomain, bool>> filterExpression, 
            ISortingParameters sorting, 
            IOptionsParameters options
        ) where TEntity : class
        {

            return DbContext.Set<TEntity>()
                            .Filter(FilterMap<TDomain, TEntity>(filterExpression))
                            .OrderBy(SortingMap<TDomain, TEntity>(sorting))
                            .ProjectTo<TEntity, TDestination>(Mapper, OptionsMap<TDomain, TEntity>(options));
        }
    }
}