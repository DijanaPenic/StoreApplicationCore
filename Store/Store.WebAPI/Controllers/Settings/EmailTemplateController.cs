﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Store.Common.Enums;
using Store.Common.Parameters;
using Store.Service.Common.Services;
using Store.Model.Common.Models;
using Store.WebAPI.Models.Settings;
using Store.WebAPI.Infrastructure.Authorization.Attributes;

namespace Store.WebAPI.Controllers
{
    // NOTE: authorized users must login to administer their client templates. Client information will be retrieved from the auth cookie.
    [ApiController]
    [Route("api/email-templates")]
    public class EmailTemplateController : ApplicationControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailTemplateController
        (
            ILogger<SettingsController> logger,
            IMapper mapper,
            IEmailTemplateService emailTemplateService,
            IQueryUtilityFacade queryUtilityFacade
        ) : base(queryUtilityFacade)
        {
            _logger = logger;
            _mapper = mapper;
            _emailTemplateService = emailTemplateService;
        }

        /// <summary>Uploads a new email template file in the system. The email template file will be replaced if it already exists.</summary>
        /// <param name="file">The email template file.</param>
        /// <param name="type">The email template type.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPut]
        [SectionAuthorization(SectionType.EmailTemplate, AccessType.Create)]
        public async Task<IActionResult> PutAsync([FromForm]IFormFile file, [FromForm] EmailTemplateType type)
        {
            if (file?.Length == 0)
            {
                return BadRequest("Email template is missing.");
            }

            // Retrieve client_id for the currently logged in user
            Guid clientId = GetCurrentUserClientId();

            await using Stream templateStream = file.OpenReadStream();
            ResponseStatus result = await _emailTemplateService.AddOrUpdateEmailTemplateAsync(clientId, type, templateStream);

            return result switch
            {
                ResponseStatus.Success => Created(),
                _ => InternalServerError()
            };
        }

        /// <summary>Retrieves the email template file by identifier.</summary>
        /// <param name="emailTemplateId">The email template identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Route("{emailTemplateId:guid}")]
        [Produces("application/octet-stream")]
        [SectionAuthorization(SectionType.EmailTemplate, AccessType.Read)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid emailTemplateId)
        {
            bool emailTemplateExists = await _emailTemplateService.EmailTemplateExistsAsync(emailTemplateId);
            if (!emailTemplateExists)
            {
                return NotFound("Email Template cannot be found.");
            }

            Stream templateStream = await _emailTemplateService.FindEmailTemplateByKeyAsync(emailTemplateId);

            return File(templateStream, "application/octet-stream");
        }

        /// <summary>Retrieves email template records.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        [Produces("application/json")]
        [SectionAuthorization(SectionType.EmailTemplate, AccessType.Read)]
        public async Task<IActionResult> GetAsync()
        {
            // Retrieve client_id for the currently logged in user
            Guid clientId = GetCurrentUserClientId();

            IEnumerable<IEmailTemplate> emailTemplates = await _emailTemplateService.FindEmailTemplatesByClientIdAsync(clientId);

            if (emailTemplates != null)
            {
                return Ok(_mapper.Map<IEnumerable<EmailTemplateGetApiModel>>(emailTemplates));
            }

            return NoContent();
        }

        /// <summary>Deletes the email template record.</summary>
        /// <param name="emailTemplateId">The email template identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpDelete]
        [Route("{emailTemplateId:guid}")]
        [SectionAuthorization(SectionType.EmailTemplate, AccessType.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid emailTemplateId)
        {
            ResponseStatus result = await _emailTemplateService.DeleteEmailTemplateAsync(emailTemplateId);

            return result switch
            {
                ResponseStatus.NotFound => NotFound("Email Template cannot be found."),
                ResponseStatus.Success => NoContent(),
                _ => InternalServerError()
            };
        }
    }
}