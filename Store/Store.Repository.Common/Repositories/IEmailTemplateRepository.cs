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

        Task<IEmailTemplate> FindEmailTemplateByKeyAsync(Guid key, IOptionsParameters options);

        Task<ResponseStatus> UpdateEmailTemplateAsync(Guid key, IEmailTemplate model);

        Task<ResponseStatus> AddEmailTemplateAsync(IEmailTemplate model);

        Task<ResponseStatus> DeleteEmailTemplateByKeyAsync(Guid keykey);
    }
}