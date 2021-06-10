using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Store.Entities;
using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Parameters.Options;
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

        public async Task<IEmailTemplate> FindEmailTemplateByClientIdAsync(Guid clientId, EmailTemplateType emailTemplateType)
        {
            EmailTemplateEntity entity = await _dbSet.FirstOrDefaultAsync(et => et.ClientId == clientId && et.Type == emailTemplateType);

            return Mapper.Map<IEmailTemplate>(entity);
        }

        public async Task<IEnumerable<IEmailTemplate>> FindEmailTemplateByClientIdAsync(Guid clientId)
        {
            IEnumerable<EmailTemplateEntity> entity = await _dbSet.Where(et => et.ClientId == clientId).ToListAsync();

            return Mapper.Map<IEnumerable<IEmailTemplate>>(entity);
        }

        public Task<bool> EmailTemplateExistsAsync(Guid emailTemplateId)
        {
           return _dbSet.AnyAsync(et => et.Id == emailTemplateId);
        }

        public Task<bool> EmailTemplateExistsAsync(Guid clientId, EmailTemplateType emailTemplateType)
        {
            return _dbSet.AnyAsync(et => et.ClientId == clientId && et.Type == emailTemplateType);
        }

        public Task<IEmailTemplate> FindEmailTemplateByKeyAsync(Guid key, IOptionsParameters options)
        {
            return FindByKeyAsync<IEmailTemplate, EmailTemplateEntity>(options, key);
        }

        public Task<ResponseStatus> UpdateEmailTemplateAsync(Guid key, IEmailTemplate model)
        {
            return UpdateAsync<IEmailTemplate, EmailTemplateEntity>(model, key);
        }

        public Task<ResponseStatus> AddEmailTemplateAsync(IEmailTemplate model)
        {
            return AddAsync<IEmailTemplate, EmailTemplateEntity>(model);
        }

        public Task<ResponseStatus> DeleteEmailTemplateByKeyAsync(Guid key)
        {
            return DeleteByKeyAsync<IEmailTemplate, EmailTemplateEntity>(key);
        }
    }
}