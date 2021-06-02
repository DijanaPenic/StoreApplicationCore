using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.Entities;
using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Extensions;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Model.Common.Models.Core;

namespace Store.Repository.Core
{
    internal abstract partial class GenericRepository
    {
        protected ApplicationDbContext DbContext;

        protected IMapper Mapper { get; }

        public GenericRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            Mapper = mapper;
        }

        protected async Task<IEnumerable<TDomain>> GetAsync<TDomain, TEntity>(IOptionsParameters options) where TEntity : class
        {
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);
            IEnumerable<TEntity> entities = await DbContext.Set<TEntity>().Include(entityProperties).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(entities);
        }

        protected async Task<IEnumerable<TDomain>> GetWithProjectionAsync<TDomain, TEntity, TDestination>(IOptionsParameters options) where TEntity : class
        {
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);
            IList<TDestination> destItems = await DbContext.Set<TEntity>().ProjectTo<TEntity, TDestination>(Mapper, entityProperties).ToListAsync();

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

        protected async Task<IPagedList<TDomain>> FindWithProjectionAsync<TDomain, TEntity, TDestination>
        (
            Expression<Func<TDomain, bool>> filterExpression, 
            IPagingParameters paging, 
            ISortingParameters sorting, 
            IOptionsParameters options
        ) where TEntity : class
        {
            IPagedList<TDestination> destPagedList = await FindWithProjection<TDomain, TEntity, TDestination>(filterExpression, sorting, options).ToPagedListAsync(paging.PageNumber, paging.PageSize);

            return destPagedList.ToPagedList<TDestination, TDomain>(Mapper);
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

        // TODO - refactor
        protected async Task<IEnumerable<TDomain>> FindWithProjectionAsync<TDomain, TEntity, TDestination>
        (
            Expression<Func<TDomain, bool>> filterExpression, 
            ISortingParameters sorting, 
            IOptionsParameters options
        ) where TEntity : class
        {
            IList<TDestination> destItems = await FindWithProjection<TDomain, TEntity, TDestination>(filterExpression, sorting, options).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(destItems);
        }

        protected async Task<TDomain> FindByIdAsync<TDomain, TEntity>
        (
            Guid id, 
            IOptionsParameters options
        ) where TEntity : class, IDBPoco
        {
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);
            TEntity entity = await DbContext.Set<TEntity>().Include(entityProperties).FirstOrDefaultAsync(e => e.Id == id);

            return Mapper.Map<TDomain>(entity);
        }

        // TODO - refactor
        protected async Task<TDomain> FindByIdWithProjectionAsync<TDomain, TEntity, TDestination>
        (
            Guid id, 
            IOptionsParameters options
        ) where TDestination : IPoco where TEntity : class, IDBPoco
        {
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDestination, TEntity>(Mapper, options?.Properties);

            // TODO - Projection won't work if filter delegate is used in FirstOrDefaultAsync - getting exception
            TDestination destItem = await DbContext.Set<TEntity>()
                                                   .Where(e => e.Id == id)
                                                   .ProjectTo<TEntity, TDestination>(Mapper, entityProperties)
                                                   .FirstOrDefaultAsync();

            return Mapper.Map<TDomain>(destItem);
        }

        protected Task<ResponseStatus> AddAsync<TDomain, TEntity>(TDomain model)
        where TDomain : class, IPoco
        where TEntity : class, IDBPoco
        {
            if (model == null)
                Task.FromResult(ResponseStatus.Error);

            TEntity entity = Mapper.Map<TEntity>(model);

            entity.Id = GuidHelper.NewSequentialGuid();
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            DbContext.Set<TEntity>().Add(entity);

            return Task.FromResult(ResponseStatus.Success);
        }

        protected async Task<ResponseStatus> DeleteByIdAsync<TDomain, TEntity>(Guid id) where TEntity : class, IDBPoco
        {
            TEntity entity = await DbContext.Set<TEntity>().FindAsync(id);

            if (entity == null)
                return ResponseStatus.NotFound;

            DbContext.Set<TEntity>().Remove(entity);

            return ResponseStatus.Success;
        }

        protected async Task<ResponseStatus> UpdateAsync<TDomain, TEntity>
        (
            Guid id, 
            TDomain model
        ) where TDomain : class, IPoco where TEntity : class, IDBPoco
        {
            if (model == null)
                return ResponseStatus.Error;

            // TEntity entity = Mapper.Map<TEntity>(model); -> can't use this for model updates
            // Need to search context or the store because AutoMapper creates a new instance of the object during domain->entity mapping and sometimes
            // the newly created instance cannot be attached to the context (if we're already tracking the original instance)
            TEntity entity = await DbContext.Set<TEntity>().FindAsync(id);

            if (entity == null)
                return ResponseStatus.NotFound;

            // DateCreatedUtc and Id properties need to be mapped from entity. Otherwise, they'll be overriden with default values.
            model.DateUpdatedUtc = DateTime.UtcNow;
            model.DateCreatedUtc = entity.DateCreatedUtc;
            model.Id = entity.Id;

            // This works only for scalar property updates. The navigation property mapping is out of the scope for AutoMapper.
            // Also, the author is highly suggesting against the domain->entity mappings.
            Mapper.Map(model, entity);
            
            DbContext.Entry(entity).State = EntityState.Modified;

            return ResponseStatus.Success;
        }

        private IQueryable<TEntity> Find<TDomain, TEntity>
        (
            Expression<Func<TDomain, bool>> filterExpression, 
            ISortingParameters sorting, 
            IOptionsParameters options
        ) where TEntity : class
        {
            Expression<Func<TEntity, bool>> entityFiltering = Mapper.Map<Expression<Func<TEntity, bool>>>(filterExpression);
            ISortingParameters entitySorting = ModelMapperHelper.GetSortPropertyMappings<TDomain, TEntity>(Mapper, sorting);
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);

            IQueryable<TEntity> query = DbContext.Set<TEntity>().Filter(entityFiltering).Include(entityProperties).OrderBy(entitySorting);

            return query;
        }

        // TODO - refactor
        private IQueryable<TDestination> FindWithProjection<TDomain, TEntity, TDestination>
        (
            Expression<Func<TDomain, bool>> filterExpression, 
            ISortingParameters sorting, 
            IOptionsParameters options
        ) where TEntity : class
        {
            Expression<Func<TEntity, bool>> entityFiltering = Mapper.Map<Expression<Func<TEntity, bool>>>(filterExpression);
            ISortingParameters entitySorting = ModelMapperHelper.GetSortPropertyMappings<TDomain, TEntity>(Mapper, sorting);
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);

            IQueryable<TDestination> query = DbContext.Set<TEntity>()
                                                      .Filter(entityFiltering)
                                                      .OrderBy(entitySorting)
                                                      .ProjectTo<TEntity, TDestination>(Mapper, entityProperties);

            return query;
        }
    }
}