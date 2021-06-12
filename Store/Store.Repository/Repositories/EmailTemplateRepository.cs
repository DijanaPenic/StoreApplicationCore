using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using LinqKit;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.Entities;
using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Core;
using Store.Repository.Common.Repositories;
using Store.Model.Common.Models;

namespace Store.Repositories
{
    internal class EmailTemplateRepository : GenericRepository, IEmailTemplateRepository
    {
        private DbSet<EmailTemplateEntity> _dbSet => DbContext.Set<EmailTemplateEntity>();

        public EmailTemplateRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task<IPagedList<IEmailTemplate>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            ExpressionStarter<IEmailTemplate> predicate = PredicateBuilder.New<IEmailTemplate>(true);

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                predicate.And(et => et.Name.Contains(filter.SearchString));
            }

            return FindAsync<IEmailTemplate, EmailTemplateEntity>(predicate, paging, sorting, options);
        }

        public Task<IEnumerable<IEmailTemplate>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IEmailTemplate, EmailTemplateEntity>(sorting, options);
        }

        public async Task<IEmailTemplate> FindByClientIdAsync(Guid clientId, EmailTemplateType emailTemplateType)
        {
            EmailTemplateEntity entity = await _dbSet.FirstOrDefaultAsync(et => et.ClientId == clientId && et.Type == emailTemplateType);

            return Mapper.Map<IEmailTemplate>(entity);
        }

        public async Task<IEnumerable<IEmailTemplate>> FindByClientIdAsync(Guid clientId)
        {
            IEnumerable<EmailTemplateEntity> entity = await _dbSet.Where(et => et.ClientId == clientId).ToListAsync();

            return Mapper.Map<IEnumerable<IEmailTemplate>>(entity);
        }

        public Task<bool> ExistsAsync(Guid emailTemplateId)
        {
           return _dbSet.AnyAsync(et => et.Id == emailTemplateId);
        }

        public Task<bool> ExistsAsync(Guid clientId, EmailTemplateType emailTemplateType)
        {
            return _dbSet.AnyAsync(et => et.ClientId == clientId && et.Type == emailTemplateType);
        }

        public Task<IEmailTemplate> FindByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IEmailTemplate, EmailTemplateEntity>(options, key);
        }

        public Task<ResponseStatus> AddAsync(IEmailTemplate model)
        {
            return AddAsync<IEmailTemplate, EmailTemplateEntity>(model);
        }

        public Task<ResponseStatus> UpdateAsync(IEmailTemplate model)
        {
            return UpdateAsync<IEmailTemplate, EmailTemplateEntity>(model, model.Id);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(Guid key)
        {
            return DeleteByKeyAsync<IEmailTemplate, EmailTemplateEntity>(key);
        }
    }
}