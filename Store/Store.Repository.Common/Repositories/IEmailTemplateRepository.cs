using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Common.Enums;
using Store.Common.Parameters.Options;
using Store.Model.Common.Models;

namespace Store.Repository.Common.Repositories
{
    public interface IEmailTemplateRepository
    {
        Task<IEmailTemplate> FindEmailTemplateByClientIdAsync(Guid clientId, EmailTemplateType emailTemplateType);

        Task<IEnumerable<IEmailTemplate>> FindEmailTemplateByClientIdAsync(Guid clientId);

        Task<bool> EmailTemplateExistsAsync(Guid emailTemplateId);

        Task<bool> EmailTemplateExistsAsync(Guid clientId, EmailTemplateType emailTemplateType);

        Task<IEmailTemplate> FindEmailTemplateByIdAsync(Guid id, IOptionsParameters options);

        Task<ResponseStatus> UpdateEmailTemplateAsync(Guid id, IEmailTemplate model);

        Task<ResponseStatus> AddEmailTemplateAsync(IEmailTemplate model);

        Task<ResponseStatus> DeleteEmailTemplateByIdAsync(Guid id);
    }
}