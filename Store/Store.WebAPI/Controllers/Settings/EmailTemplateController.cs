using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Store.Common.Enums;
using Store.Common.Helpers.Identity;
using Store.Model.Common.Models;
using Store.WebAPI.Models.Settings;
using Store.WebAPI.Infrastructure.Attributes;
using Store.Service.Common.Services;

namespace Store.WebAPI.Controllers
{
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
            IEmailTemplateService emailTemplateService
        )
        {
            _logger = logger;
            _mapper = mapper;
            _emailTemplateService = emailTemplateService;
        }

        [HttpPost]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("")]
        public async Task<IActionResult> PostAsync([FromForm]IFormFile file, [FromForm]EmailTemplateType type)
        {
            if (file?.Length == 0)
            {
                return BadRequest("Email template is missing.");
            }

            Guid clientId = GetClientIdForCurrentlyLoggedInUser();

            using Stream templateStream = file.OpenReadStream();
            await _emailTemplateService.AddEmailTemplateAsync(templateStream, clientId, type);

            return Ok();
        }

        [HttpPatch]
        [AuthorizationFilter(RoleHelper.Admin)]
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
        [AuthorizationFilter(RoleHelper.Admin)]
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
        [AuthorizationFilter(RoleHelper.Admin)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync()
        {
            Guid clientId = GetClientIdForCurrentlyLoggedInUser();

            IEnumerable<IEmailTemplate> emailTemplates = await _emailTemplateService.FindEmailTemplatesByClientIdAsync(clientId);

            if (emailTemplates != null)
            {
                return Ok(_mapper.Map<IEnumerable<EmailTemplateGetApiModel>>(emailTemplates));
            }

            return NoContent();
        }

        [HttpDelete]
        [AuthorizationFilter(RoleHelper.Admin)]
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

        private Guid GetClientIdForCurrentlyLoggedInUser() => Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value);
    }
}