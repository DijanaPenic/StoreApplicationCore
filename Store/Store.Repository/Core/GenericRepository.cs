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
using Store.Repository.Common.Core;
using Store.Model.Common.Models.Core;

namespace Store.Repository.Core
{
    public class GenericRepository<TEntity, TDomain> : IGenericRepository<TDomain> where TDomain : class, IPoco where TEntity : class, IDBPoco
    {
        private DbSet<TEntity> _set;

        protected DbSet<TEntity> Set => _set ??= DbContext.Set<TEntity>();

        protected IMapper Mapper { get; }

        protected ApplicationDbContext DbContext { get; }

        public GenericRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext ?? throw new ArgumentNullException("GenericRepository - database context is missing.");
            Mapper = mapper ?? throw new ArgumentNullException("GenericRepository - mapper is missing.");
        }

        public async Task<IEnumerable<TDomain>> GetAsync(params string[] includeProperties)
        {
            string[] entityIncludeProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, includeProperties);
            IEnumerable<TEntity> entities = await Set.Include(entityIncludeProperties).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(entities);
        }

        public async Task<IEnumerable<TDomain>> GetWithProjectionAsync<TDestination>(params string[] includeProperties)
        {
            string[] entityIncludeProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, includeProperties);
            IList<TDestination> destItems = await Set.ProjectTo<TEntity, TDestination>(Mapper, entityIncludeProperties).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(destItems);
        }

        public async Task<IPagedList<TDomain>> FindAsync(Expression<Func<TDomain, bool>> filterExpression, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, params string[] includeProperties)
        {
            IPagedList<TEntity> entityPagedList = await Find(filterExpression, sortOrderProperty, isDescendingSortOrder, includeProperties).ToPagedListAsync(pageNumber, pageSize);

            return entityPagedList.ToPagedList<TEntity, TDomain>(Mapper);
        }

        public async Task<IPagedList<TDomain>> FindWithProjectionAsync<TDestination>(Expression<Func<TDomain, bool>> filterExpression, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, params string[] includeProperties)
        {
            IPagedList<TDestination> destPagedList = await FindWithProjection<TDestination>(filterExpression, sortOrderProperty, isDescendingSortOrder, includeProperties).ToPagedListAsync(pageNumber, pageSize);

            return destPagedList.ToPagedList<TDestination, TDomain>(Mapper);
        }

        public async Task<IEnumerable<TDomain>> FindAsync(Expression<Func<TDomain, bool>> filterExpression, string sortOrderProperty, bool isDescendingSortOrder, params string[] includeProperties) 
        {
            IEnumerable<TEntity> entities = await Find(filterExpression, sortOrderProperty, isDescendingSortOrder, includeProperties).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(entities);
        }

        public async Task<IEnumerable<TDomain>> FindWithProjectionAsync<TDestination>(Expression<Func<TDomain, bool>> filterExpression, string sortOrderProperty, bool isDescendingSortOrder, params string[] includeProperties)
        {
            IList<TDestination> destItems = await FindWithProjection<TDestination>(filterExpression, sortOrderProperty, isDescendingSortOrder, includeProperties).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(destItems);
        }

        public async Task<TDomain> FindByIdAsync(Guid id, params string[] includeProperties)
        {
            string[] entityIncludeProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, includeProperties);
            TEntity entity = await Set.Include(entityIncludeProperties).FirstOrDefaultAsync(e => e.Id == id);

            return Mapper.Map<TDomain>(entity);
        }

        public async Task<TDomain> FindByIdWithProjectionAsync<TDestination>(Guid id, params string[] includeProperties) where TDestination : IPoco
        {
            string[] entityIncludeProperties = ModelMapperHelper.GetPropertyMappings<TDestination, TEntity>(Mapper, includeProperties);

            // TODO - Projection won't work if filter delegate is used in FirstOrDefaultAsync - getting exception
            TDestination destItem = await Set.Where(e => e.Id == id).ProjectTo<TEntity, TDestination>(Mapper, entityIncludeProperties).FirstOrDefaultAsync(); 

            return Mapper.Map<TDomain>(destItem);
        }

        public Task<ResponseStatus> AddAsync(TDomain model)
        {
            if (model == null)
                Task.FromResult(ResponseStatus.Error);

            TEntity entity = Mapper.Map<TEntity>(model);

            entity.Id = GuidHelper.NewSequentialGuid();
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Set.Add(entity);

            return Task.FromResult(ResponseStatus.Success);
        }

        public async Task<ResponseStatus> DeleteByIdAsync(Guid id)
        {
            TEntity entity = await Set.FindAsync(id);

            if (entity == null)
                return ResponseStatus.NotFound;

            Set.Remove(entity);

            return ResponseStatus.Success;
        }

        public async Task<ResponseStatus> UpdateAsync(Guid id, TDomain model)
        {
            if (model == null)
                return ResponseStatus.Error;

            TEntity entity = await Set.FindAsync(id);

            if (entity == null)
                return ResponseStatus.NotFound;

            // DateUpdatedUtc and Id properties need to be mapped from entity. Otherwise, they'll be overriden with default values.
            model.DateUpdatedUtc = DateTime.UtcNow;
            model.DateCreatedUtc = entity.DateCreatedUtc;
            model.Id = entity.Id;

            Mapper.Map(model, entity);
            
            DbContext.Entry(entity).State = EntityState.Modified;

            return ResponseStatus.Success;
        }

        public async Task<ResponseStatus> UpdateAsync(TDomain model)
        {
            if (model == null)
                return ResponseStatus.Error;

            // TEntity entity = Mapper.Map<TEntity>(model); -> can't use this for model updates
            // Need to search context or the store because AutoMapper creates a new instance of the object during domain->entity mapping and sometimes
            // the newly created instance cannot be attached to the context (if we're already tracking the original instance)
            TEntity entity = await Set.FindAsync(model.Id);

            if (entity == null)
                return ResponseStatus.NotFound;

            // This works only for scalar property updates. The navigation property mapping is out of the scope for AutoMapper.
            // Also, the author is highly suggesting against the domain->entity mappings.
            Mapper.Map(model, entity);

            entity.DateUpdatedUtc = DateTime.UtcNow;

            DbContext.Entry(entity).State = EntityState.Modified;

            return ResponseStatus.Success;
        }

        private IQueryable<TEntity> Find(Expression<Func<TDomain, bool>> filterExpression, string sortOrderProperty, bool isDescendingSortOrder, params string[] includeProperties)
        {
            Expression<Func<TEntity, bool>> entityFilterExpression = Mapper.Map<Expression<Func<TEntity, bool>>>(filterExpression);
            string entitySortOrderProperty = ModelMapperHelper.GetPropertyMapping<TDomain, TEntity>(Mapper, sortOrderProperty);
            string[] entityIncludeProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, includeProperties);

            IQueryable<TEntity> query = Set.Filter(entityFilterExpression)
                                           .Include(entityIncludeProperties)
                                           .OrderBy(entitySortOrderProperty, isDescendingSortOrder);

            return query;
        }

        private IQueryable<TDestination> FindWithProjection<TDestination>(Expression<Func<TDomain, bool>> filterExpression, string sortOrderProperty, bool isDescendingSortOrder, params string[] includeProperties)
        {
            Expression<Func<TEntity, bool>> entityFilterExpression = Mapper.Map<Expression<Func<TEntity, bool>>>(filterExpression);
            string entitySortOrderProperty = ModelMapperHelper.GetPropertyMapping<TDomain, TEntity>(Mapper, sortOrderProperty);
            string[] entityIncludeProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, includeProperties);

            IQueryable<TDestination> query = Set.Filter(entityFilterExpression)
                                                .OrderBy(entitySortOrderProperty, isDescendingSortOrder)
                                                .ProjectTo<TEntity, TDestination>(Mapper, entityIncludeProperties);

            return query;
        }
    }
}