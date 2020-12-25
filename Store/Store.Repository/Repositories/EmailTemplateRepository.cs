﻿using System;
using System.Threading.Tasks;
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
            EmailTemplateEntity entity = await Set.FirstOrDefaultAsync(et => et.ClientId == clientId && et.Type == emailTemplateType);

            return Mapper.Map<IEmailTemplate>(entity);
        }
    }
}