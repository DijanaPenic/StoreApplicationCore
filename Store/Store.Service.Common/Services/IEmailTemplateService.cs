﻿using System;
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

        Task<Stream> FindEmailTemplateByIdAsync(Guid emailTemplateId);

        Task<IEnumerable<IEmailTemplate>> FindEmailTemplatesByClientIdAsync(Guid clientId);

        Task<ResponseStatus> UpdateEmailTemplateAsync(Guid emailTemplateId, Stream templateStream);

        Task<ResponseStatus> AddEmailTemplateAsync(Stream templateStream, Guid clientId, EmailTemplateType templateType);

        Task<ResponseStatus> DeleteEmailTemplateAsync(Guid emailTemplateId);
    }
}