using System;
using System.Threading.Tasks;

using Store.Common.Enums;
using Store.Model.Common.Models;
using Store.Repository.Common.Core;

namespace Store.Repository.Common.Repositories
{
    public interface IEmailTemplateRepository : IGenericRepository<IEmailTemplate>
    {
        Task<IEmailTemplate> FindByClientIdAsync(Guid clientId, EmailTemplateType emailTemplateType);
    }
}