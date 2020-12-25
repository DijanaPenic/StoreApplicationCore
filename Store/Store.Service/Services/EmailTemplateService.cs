using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using X.PagedList;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Repository.Common.Core;
using Store.Service.Common.Services;

namespace Store.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmailTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEmailTemplate> FindEmailTemplateAsync(Guid clientId, EmailTemplateType emailTemplateType)
        {
            return _unitOfWork.EmailTemplateRepository.FindByClientIdAsync(clientId, emailTemplateType);
        }

        public async Task<ResponseStatus> UpdateEmailTemplateAsync(IEmailTemplate emailTemplate)
        {
            ResponseStatus status = await _unitOfWork.EmailTemplateRepository.UpdateAsync(emailTemplate);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> AddEmailTemplateAsync(IEmailTemplate emailTemplate)
        {
            ResponseStatus status = await _unitOfWork.EmailTemplateRepository.AddAsync(emailTemplate);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> DeleteEmailTemplateAsync(Guid emailTemplateId)
        {
            ResponseStatus status = await _unitOfWork.EmailTemplateRepository.DeleteByIdAsync(emailTemplateId);

            return await _unitOfWork.SaveChangesAsync(status);
        }
    }
}