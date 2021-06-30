using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Common.Enums;
using Store.Model.Common.Models;

namespace Store.Service.Common.Services
{
    public interface IEmailTemplateService
    {
        Task<bool> EmailTemplateExistsAsync(Guid emailTemplateId);

        Task<bool> EmailTemplateExistsAsync(Guid clientId, EmailTemplateType emailTemplateType);

        Task<Stream> FindEmailTemplateByKeyAsync(Guid emailTemplateId);

        Task<IEnumerable<IEmailTemplate>> FindEmailTemplatesByClientIdAsync(Guid clientId);

        Task<Stream> FindEmailTemplateByClientIdAsync(Guid clientId, EmailTemplateType templateType);

        Task<ResponseStatus> AddOrUpdateEmailTemplateAsync(Guid clientId, EmailTemplateType templateType, Stream templateStream);

        Task<ResponseStatus> DeleteEmailTemplateAsync(Guid emailTemplateId);
    }
}