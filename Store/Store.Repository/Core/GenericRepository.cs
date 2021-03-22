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
using Store.Repository.Common.Core;
using Store.Model.Common.Models.Core;

namespace Store.Repository.Core
{
    public class GenericRepository<TEntity, TDomain> : IGenericRepository<TDomain> where TDomain : class, IPoco where TEntity : class, IDBPoco
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<TEntity> _dbSet;

        protected DbSet<TEntity> DbSet => _dbSet ??= _dbContext.Set<TEntity>();

        protected IMapper Mapper { get; }

        public GenericRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            Mapper = mapper;
        }

        public async Task<IEnumerable<TDomain>> GetAsync(IOptionsParameters options)
        {
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);
            IEnumerable<TEntity> entities = await DbSet.Include(entityProperties).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(entities);
        }

        public async Task<IEnumerable<TDomain>> GetWithProjectionAsync<TDestination>(IOptionsParameters options)
        {
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);
            IList<TDestination> destItems = await DbSet.ProjectTo<TEntity, TDestination>(Mapper, entityProperties).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(destItems);
        }

        public async Task<IPagedList<TDomain>> FindAsync(Expression<Func<TDomain, bool>> filterExpression, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            IPagedList<TEntity> entityPagedList = await Find(filterExpression, sorting, options).ToPagedListAsync(paging.PageNumber, paging.PageSize);

            return entityPagedList.ToPagedList<TEntity, TDomain>(Mapper);
        }

        public async Task<IPagedList<TDomain>> FindWithProjectionAsync<TDestination>(Expression<Func<TDomain, bool>> filterExpression, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            IPagedList<TDestination> destPagedList = await FindWithProjection<TDestination>(filterExpression, sorting, options).ToPagedListAsync(paging.PageNumber, paging.PageSize);

            return destPagedList.ToPagedList<TDestination, TDomain>(Mapper);
        }

        public async Task<IEnumerable<TDomain>> FindAsync(Expression<Func<TDomain, bool>> filterExpression, ISortingParameters sorting, IOptionsParameters options) 
        {
            IEnumerable<TEntity> entities = await Find(filterExpression, sorting, options).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(entities);
        }

        public async Task<IEnumerable<TDomain>> FindWithProjectionAsync<TDestination>(Expression<Func<TDomain, bool>> filterExpression, ISortingParameters sorting, IOptionsParameters options)
        {
            IList<TDestination> destItems = await FindWithProjection<TDestination>(filterExpression, sorting, options).ToListAsync();

            return Mapper.Map<IEnumerable<TDomain>>(destItems);
        }

        public async Task<TDomain> FindByIdAsync(Guid id, IOptionsParameters options)
        {
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);
            TEntity entity = await DbSet.Include(entityProperties).FirstOrDefaultAsync(e => e.Id == id);

            return Mapper.Map<TDomain>(entity);
        }

        public async Task<TDomain> FindByIdWithProjectionAsync<TDestination>(Guid id, IOptionsParameters options) where TDestination : IPoco
        {
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDestination, TEntity>(Mapper, options?.Properties);

            // TODO - Projection won't work if filter delegate is used in FirstOrDefaultAsync - getting exception
            TDestination destItem = await DbSet.Where(e => e.Id == id).ProjectTo<TEntity, TDestination>(Mapper, entityProperties).FirstOrDefaultAsync();

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

            DbSet.Add(entity);

            return Task.FromResult(ResponseStatus.Success);
        }

        public async Task<ResponseStatus> DeleteByIdAsync(Guid id)
        {
            TEntity entity = await DbSet.FindAsync(id);

            if (entity == null)
                return ResponseStatus.NotFound;

            DbSet.Remove(entity);

            return ResponseStatus.Success;
        }

        public async Task<ResponseStatus> UpdateAsync(Guid id, TDomain model)
        {
            if (model == null)
                return ResponseStatus.Error;

            // TEntity entity = Mapper.Map<TEntity>(model); -> can't use this for model updates
            // Need to search context or the store because AutoMapper creates a new instance of the object during domain->entity mapping and sometimes
            // the newly created instance cannot be attached to the context (if we're already tracking the original instance)
            TEntity entity = await DbSet.FindAsync(id);

            if (entity == null)
                return ResponseStatus.NotFound;

            // DateCreatedUtc and Id properties need to be mapped from entity. Otherwise, they'll be overriden with default values.
            model.DateUpdatedUtc = DateTime.UtcNow;
            model.DateCreatedUtc = entity.DateCreatedUtc;
            model.Id = entity.Id;

            // This works only for scalar property updates. The navigation property mapping is out of the scope for AutoMapper.
            // Also, the author is highly suggesting against the domain->entity mappings.
            Mapper.Map(model, entity);
            
            _dbContext.Entry(entity).State = EntityState.Modified;

            return ResponseStatus.Success;
        }

        public Task<ResponseStatus> UpdateAsync(TDomain model)
        {
            return UpdateAsync(model.Id, model); // TODO - potentially omit this method
        }

        private IQueryable<TEntity> Find(Expression<Func<TDomain, bool>> filterExpression, ISortingParameters sorting, IOptionsParameters options)
        {
            Expression<Func<TEntity, bool>> entityFiltering = Mapper.Map<Expression<Func<TEntity, bool>>>(filterExpression);
            ISortingParameters entitySorting = ModelMapperHelper.GetSortPropertyMappings<TDomain, TEntity>(Mapper, sorting);
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);

            IQueryable<TEntity> query = DbSet.Filter(entityFiltering).Include(entityProperties).OrderBy(entitySorting);

            return query;
        }

        private IQueryable<TDestination> FindWithProjection<TDestination>(Expression<Func<TDomain, bool>> filterExpression, ISortingParameters sorting, IOptionsParameters options)
        {
            Expression<Func<TEntity, bool>> entityFiltering = Mapper.Map<Expression<Func<TEntity, bool>>>(filterExpression);
            ISortingParameters entitySorting = ModelMapperHelper.GetSortPropertyMappings<TDomain, TEntity>(Mapper, sorting);
            string[] entityProperties = ModelMapperHelper.GetPropertyMappings<TDomain, TEntity>(Mapper, options?.Properties);

            IQueryable<TDestination> query = DbSet.Filter(entityFiltering).OrderBy(entitySorting).ProjectTo<TEntity, TDestination>(Mapper, entityProperties);

            return query;
        }
    }
}