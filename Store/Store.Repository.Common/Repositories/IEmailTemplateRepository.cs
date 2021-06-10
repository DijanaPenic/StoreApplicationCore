using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Common.Enums;
using Store.Model.Common.Models;

namespace Store.Repository.Common.Repositories
{
    public interface IEmailTemplateRepository : IRepository<IEmailTemplate, Guid>
    {
        Task<IEmailTemplate> FindByClientIdAsync(Guid clientId, EmailTemplateType emailTemplateType);

        Task<IEnumerable<IEmailTemplate>> FindByClientIdAsync(Guid clientId);

        Task<bool> ExistsAsync(Guid emailTemplateId);

        Task<bool> ExistsAsync(Guid clientId, EmailTemplateType emailTemplateType);
    }
}