using System;
using System.IO;
using System.Threading.Tasks;

using Store.Common.Enums;
using Store.Model.Common.Models;

namespace Store.Service.Common.Services
{
    public interface IEmailTemplateService
    {
        Task<IEmailTemplate> FindEmailTemplateAsync(Guid clientId, EmailTemplateType emailTemplateType);

        Task<IEmailTemplate> FindEmailTemplateByIdAsync(Guid emailTemplateId);

        Task<ResponseStatus> UpdateEmailTemplateAsync(IEmailTemplate emailTemplate, Stream templateStream);

        Task<ResponseStatus> AddEmailTemplateAsync(Stream templateStream, Guid clientId, EmailTemplateType templateType);

        Task<ResponseStatus> DeleteEmailTemplateAsync(Guid emailTemplateId);
    }
}