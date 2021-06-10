using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Extensions;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Core;
using Store.Repository.Extensions;
using Store.Repository.Common.Repositories.Identity;
using Store.Model.Common.Models.Identity;
using Store.Entities.Identity;

namespace Store.Repositories.Identity
{
    internal class RoleRepository : GenericRepository, IRoleRepository
    {
        private DbSet<RoleEntity> _dbSet => DbContext.Set<RoleEntity>();

        public RoleRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public Task<IPagedList<IRole>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            Expression<Func<IRole, bool>> filterExpression = string.IsNullOrEmpty(filter.SearchString) ? null : r => r.Name.Contains(filter.SearchString);

            return FindAsync<IRole, RoleEntity>(filterExpression, paging, sorting, options);
        }

        public Task<IEnumerable<IRole>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IRole, RoleEntity>(sorting, options);
        }

        public Task<IRole> FindByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IRole, RoleEntity>(options, key);
        }

        public Task<ResponseStatus> UpdateAsync(IRole model)
        {
            return UpdateAsync<IRole, RoleEntity>(model, model.Id);
        }

        public Task<ResponseStatus> AddAsync(IRole model)
        {
            return AddAsync<IRole, RoleEntity>(model);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(Guid key)
        {
            return DeleteByKeyAsync<IRole, RoleEntity>(key);
        }

        public async Task<IPagedList<IRole>> FindBySectionAsync(IPermissionFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting)
        {
            string sectionType = $"{filter.SectionType}.";

            IPagedList<RoleEntity> entityPagedList = await _dbSet.Filter(string.IsNullOrEmpty(filter.SearchString) ? null : r => r.Name.Contains(filter.SearchString))
                                                                 .Filter(r => r.Claims.Any(rc => rc.ClaimValue.StartsWith(sectionType)))
                                                                 .Include(r => r.Claims.Where(rc => rc.ClaimValue.StartsWith(sectionType)))
                                                                 .OrderBy(SortingMap<IRole, RoleEntity>(sorting))
                                                                 .ToPagedListAsync(paging.PageNumber, paging.PageSize);

            return entityPagedList.ToPagedList<RoleEntity, IRole>(Mapper);
        }

        public async Task<IRole> FindByNameAsync(string roleName)
        {
            RoleEntity entity = await _dbSet.Where(r => r.NormalizedName == roleName).SingleOrDefaultAsync();

            return Mapper.Map<IRole>(entity);
        }

        public async Task<IEnumerable<IRole>> FindByNameAsync(string[] roleNames)
        {
            IEnumerable<RoleEntity> entities = await _dbSet.Where(r => roleNames.Contains(r.NormalizedName)).ToListAsync();

            return Mapper.Map<IEnumerable<IRole>>(entities);
        }
    }
}