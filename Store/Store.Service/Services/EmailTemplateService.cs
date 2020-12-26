using System;
using System.IO;
using System.Threading.Tasks;

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

        public Task<IEmailTemplate> FindEmailTemplateAsync(Guid clientId, EmailTemplateType emailTemplateType)
        {
            return _unitOfWork.EmailTemplateRepository.FindByClientIdAsync(clientId, emailTemplateType);
        }

        public Task<IEmailTemplate> FindEmailTemplateByIdAsync(Guid emailTemplateId)
        {
            return _unitOfWork.EmailTemplateRepository.FindByIdAsync(emailTemplateId);
        }

        public async Task<ResponseStatus> UpdateEmailTemplateAsync(IEmailTemplate emailTemplate, Stream templateStream)
        {
            await _fileProvider.SaveFileAsync(emailTemplate.ClientId.ToString(), $"templates\\{emailTemplate.Name}", templateStream);

            ResponseStatus status = await _unitOfWork.EmailTemplateRepository.UpdateAsync(emailTemplate);

            return await _unitOfWork.SaveChangesAsync(status);
        }

        public async Task<ResponseStatus> AddEmailTemplateAsync(Stream templateStream, Guid clientId, EmailTemplateType templateType)
        {
            string templateName = $"{templateType.ToString().ToSnakeCase()}.html";
            string filePath = await _fileProvider.SaveFileAsync(clientId.ToString(), $"templates\\{templateName}", templateStream);

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
    }
}