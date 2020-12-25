using System;
using System.Threading.Tasks;

using Store.Common.Enums;
using Store.Model.Common.Models;

namespace Store.Service.Common.Services
{
    public interface IEmailTemplateService
    {
        Task<IEmailTemplate> FindEmailTemplateAsync(Guid clientId, EmailTemplateType emailTemplateType);

        Task<ResponseStatus> UpdateEmailTemplateAsync(IEmailTemplate emailTemplate);

        Task<ResponseStatus> AddEmailTemplateAsync(IEmailTemplate emailTemplate);

        Task<ResponseStatus> DeleteEmailTemplateAsync(Guid emailTemplateId);
    }
}