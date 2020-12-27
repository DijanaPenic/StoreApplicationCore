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
    public class EmailTemplateService : IEmailTemplateService
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
            return _unitOfWork.EmailTemplateRepository.ExistsAsync(emailTemplateId);
        }

        public async Task<Stream> FindEmailTemplateByIdAsync(Guid emailTemplateId)
        {
            IEmailTemplate emailTemplate = await _unitOfWork.EmailTemplateRepository.FindByIdAsync(emailTemplateId);

            Stream templateStream = await _fileProvider.GetFileAsync(emailTemplate.ClientId.ToString(), GetEmailTemplatePath(emailTemplate.Name));

            return templateStream;
        }

        public Task<IEnumerable<IEmailTemplate>> FindEmailTemplatesByClientIdAsync(Guid clientId)
        {
            return _unitOfWork.EmailTemplateRepository.FindByClientIdAsync(clientId);
        }

        public async Task<ResponseStatus> UpdateEmailTemplateAsync(Guid emailTemplateId, Stream templateStream)
        {
            IEmailTemplate emailTemplate = await _unitOfWork.EmailTemplateRepository.FindByIdAsync(emailTemplateId);

            await _fileProvider.SaveFileAsync(emailTemplate.ClientId.ToString(), GetEmailTemplatePath(emailTemplate.Name), templateStream);

            ResponseStatus status = await _unitOfWork.EmailTemplateRepository.UpdateAsync(emailTemplate);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> AddEmailTemplateAsync(Stream templateStream, Guid clientId, EmailTemplateType templateType)
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

            ResponseStatus status = await _unitOfWork.EmailTemplateRepository.AddAsync(emailTemplate);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> DeleteEmailTemplateAsync(Guid emailTemplateId)
        {
            ResponseStatus status = await _unitOfWork.EmailTemplateRepository.DeleteByIdAsync(emailTemplateId);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        private static string GetEmailTemplatePath(string fileName) => $"templates\\{fileName}";
    }
}