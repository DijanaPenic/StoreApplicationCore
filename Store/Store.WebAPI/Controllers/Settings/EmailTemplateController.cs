using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Store.Common.Enums;
using Store.Common.Helpers.Identity;
using Store.Services.Identity;
using Store.Service.Common.Services;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.WebAPI.Models.Settings;
using Store.WebAPI.Infrastructure.Attributes;

namespace Store.WebAPI.Controllers
{
    // NOTE: admin must login to administer client templates. Client information will be retrieved from the auth cookie.
    [ApiController]
    [Route("api/email-templates")]
    public class EmailTemplateController : ApplicationControllerBase
    {
        private readonly ApplicationAuthManager _authManager;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailTemplateController
        (
            ApplicationAuthManager authManager,
            ILogger<SettingsController> logger,
            IMapper mapper,
            IEmailTemplateService emailTemplateService
        )
        {
            _authManager = authManager;
            _logger = logger;
            _mapper = mapper;
            _emailTemplateService = emailTemplateService;
        }

        [HttpPost]
        [UserAuthorization(RoleHelper.Admin)]
        [Route("")]
        public async Task<IActionResult> PostAsync([FromForm]IFormFile file, [FromForm]EmailTemplateType type)
        {
            if (file?.Length == 0)
            {
                return BadRequest("Email template is missing.");
            }

            // Retrieve client_id for the currently logged in user
            Guid clientId = GetCurrentUserClientId();

            IClient client = await _authManager.GetClientByIdAsync(clientId);
            if (client == null)
            {
                return NotFound("Client not found.");
            }

            bool emailTemplateExists = await _emailTemplateService.EmailTemplateExistsAsync(clientId, type);
            if(emailTemplateExists)
            {
                return BadRequest("Email template already exists.");
            }

            using Stream templateStream = file.OpenReadStream();
            await _emailTemplateService.AddEmailTemplateAsync(clientId, type, templateStream);

            return Ok();
        }

        [HttpPatch]
        [UserAuthorization(RoleHelper.Admin)]
        [Route("{emailTemplateId:guid}")]
        public async Task<IActionResult> PatchAsync([FromRoute] Guid emailTemplateId, [FromForm] IFormFile file)
        {
            if (file?.Length == 0)
            {
                return BadRequest("Email template is missing.");
            }

            if (emailTemplateId == Guid.Empty)
            {
                return BadRequest();
            }

            bool emailTemplateExists = await _emailTemplateService.EmailTemplateExistsAsync(emailTemplateId);
            if (!emailTemplateExists)
            {
                return NotFound();
            }

            using Stream templateStream = file.OpenReadStream();
            await _emailTemplateService.UpdateEmailTemplateAsync(emailTemplateId, templateStream);

            return Ok();
        }

        [HttpGet]
        [UserAuthorization(RoleHelper.Admin)]
        [Route("{emailTemplateId:guid}")]
        [Produces("application/octet-stream")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid emailTemplateId)
        {
            if (emailTemplateId == Guid.Empty)
            {
                return BadRequest();
            }

            bool emailTemplateExists = await _emailTemplateService.EmailTemplateExistsAsync(emailTemplateId);
            if (!emailTemplateExists)
            {
                return NotFound();
            }

            Stream templateStream = await _emailTemplateService.FindEmailTemplateByIdAsync(emailTemplateId);

            return File(templateStream, "application/octet-stream");
        }

        [HttpGet]
        [UserAuthorization(RoleHelper.Admin)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync()
        {
            // Retrieve client_id for the currently logged in user
            Guid clientId = GetCurrentUserClientId();

            IClient client = await _authManager.GetClientByIdAsync(clientId);
            if (client == null)
            {
                return NotFound("Client not found.");
            }

            IEnumerable<IEmailTemplate> emailTemplates = await _emailTemplateService.FindEmailTemplatesByClientIdAsync(clientId);

            if (emailTemplates != null)
            {
                return Ok(_mapper.Map<IEnumerable<EmailTemplateGetApiModel>>(emailTemplates));
            }

            return NoContent();
        }

        [HttpDelete]
        [UserAuthorization(RoleHelper.Admin)]
        [Route("{emailTemplateId:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid emailTemplateId)
        {
            if (emailTemplateId == Guid.Empty)
            {
                return BadRequest();
            }

            bool emailTemplateExists = await _emailTemplateService.EmailTemplateExistsAsync(emailTemplateId);
            if (!emailTemplateExists)
            {
                return NotFound();
            }

            await _emailTemplateService.DeleteEmailTemplateAsync(emailTemplateId);

            return Ok();
        }
    }
}