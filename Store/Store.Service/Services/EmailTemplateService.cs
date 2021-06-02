using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Common.Extensions;
using Store.Repository.Common.Core;
using Store.Service.Common.Services;
using Store.FileProvider.Common.Core;

namespace Store.Services
{
    internal class EmailTemplateService : IEmailTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileProvider _fileProvider;

        public EmailTemplateService(IUnitOfWork unitOfWork, IFileProvider fileProvider)
        {
            _unitOfWork = unitOfWork;
            _fileProvider = fileProvider;
        }

        public Task<bool> EmailTemplateExistsAsync(Guid emailTemplateId)
        {
            return _unitOfWork.EmailTemplateRepository.EmailTemplateExistsAsync(emailTemplateId);
        }

        public Task<bool> EmailTemplateExistsAsync(Guid clientId, EmailTemplateType emailTemplateType)
        {
            return _unitOfWork.EmailTemplateRepository.EmailTemplateExistsAsync(clientId, emailTemplateType);
        }

        public async Task<Stream> FindEmailTemplateByIdAsync(Guid emailTemplateId)
        {
            IEmailTemplate emailTemplate = await _unitOfWork.EmailTemplateRepository.FindEmailTemplateByIdAsync(emailTemplateId, null);
            if (emailTemplate == null) return default;

            return await _fileProvider.GetFileAsync(emailTemplate.ClientId.ToString(), GetEmailTemplatePath(emailTemplate.Name));
        }

        public Task<IEnumerable<IEmailTemplate>> FindEmailTemplatesByClientIdAsync(Guid clientId)
        {
            return _unitOfWork.EmailTemplateRepository.FindEmailTemplateByClientIdAsync(clientId);
        }

        public async Task<Stream> FindEmailTemplateByClientIdAsync(Guid clientId, EmailTemplateType templateType)
        {
            IEmailTemplate emailTemplate = await _unitOfWork.EmailTemplateRepository.FindEmailTemplateByClientIdAsync(clientId, templateType);
            if (emailTemplate == null) return default;

            return await _fileProvider.GetFileAsync(emailTemplate.ClientId.ToString(), GetEmailTemplatePath(emailTemplate.Name));
        }

        public async Task<ResponseStatus> UpdateEmailTemplateAsync(Guid emailTemplateId, Stream templateStream)
        {
            IEmailTemplate emailTemplate = await _unitOfWork.EmailTemplateRepository.FindEmailTemplateByIdAsync(emailTemplateId, null);

            await _fileProvider.SaveFileAsync(emailTemplate.ClientId.ToString(), GetEmailTemplatePath(emailTemplate.Name), templateStream);

            ResponseStatus status = await _unitOfWork.EmailTemplateRepository.UpdateEmailTemplateAsync(emailTemplate.Id, emailTemplate);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<ResponseStatus> AddEmailTemplateAsync(Guid clientId, EmailTemplateType templateType, Stream templateStream)
        {
            string templateName = $"{templateType.ToString().ToSnakeCase()}.html";
            string filePath = await _fileProvider.SaveFileAsync(clientId.ToString(), GetEmailTemplatePath(templateName), templateStream);

            IEmailTemplate emailTemplate = new EmailTemplate
            {
                ClientId = clientId,
                Path = filePath,
                Name = templateName,
                Type = templateType
            };

            ResponseStatus status = await _unitOfWork.EmailTemplateRepository.AddEmailTemplateAsync(emailTemplate);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<ResponseStatus> DeleteEmailTemplateAsync(Guid emailTemplateId)
        {
            ResponseStatus status = await _unitOfWork.EmailTemplateRepository.DeleteEmailTemplateByIdAsync(emailTemplateId);
            if (status != ResponseStatus.Success) return status;

            return await _unitOfWork.CommitAsync();
        }

        private static string GetEmailTemplatePath(string fileName) => $"templates\\{fileName}";
    }
}