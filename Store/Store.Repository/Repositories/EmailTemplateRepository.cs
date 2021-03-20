using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Store.Entities;
using Store.DAL.Context;
using Store.Common.Enums;
using Store.Repository.Core;
using Store.Repository.Common.Repositories;
using Store.Model.Common.Models;

namespace Store.Repositories
{
    public class EmailTemplateRepository : GenericRepository<EmailTemplateEntity, IEmailTemplate>, IEmailTemplateRepository
    {
        public EmailTemplateRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<IEmailTemplate> FindByClientIdAsync(Guid clientId, EmailTemplateType emailTemplateType)
        {
            EmailTemplateEntity entity = await DbSet.FirstOrDefaultAsync(et => et.ClientId == clientId && et.Type == emailTemplateType);

            return Mapper.Map<IEmailTemplate>(entity);
        }

        public async Task<IEnumerable<IEmailTemplate>> FindByClientIdAsync(Guid clientId)
        {
            IEnumerable<EmailTemplateEntity> entity = await DbSet.Where(et => et.ClientId == clientId).ToListAsync();

            return Mapper.Map<IEnumerable<IEmailTemplate>>(entity);
        }

        public Task<bool> ExistsAsync(Guid emailTemplateId)
        {
           return DbSet.AnyAsync(et => et.Id == emailTemplateId);
        }

        public Task<bool> ExistsAsync(Guid clientId, EmailTemplateType emailTemplateType)
        {
            return DbSet.AnyAsync(et => et.ClientId == clientId && et.Type == emailTemplateType);
        }
    }
}