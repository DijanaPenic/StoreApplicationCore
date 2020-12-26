using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Store.Common.Enums;
using Store.Common.Helpers.Identity;
using Store.Model.Common.Models;
using Store.Service.Common.Services;
using Store.WebAPI.Infrastructure.Attributes;

namespace Store.WebAPI.Controllers
{
    [ApiController]
    [Route("api/email-template")]
    public class EmailTemplateController : ApplicationControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailTemplateController
        (
            ILogger<SettingsController> logger,
            IEmailTemplateService emailTemplateService
        )
        {
            _logger = logger;
            _emailTemplateService = emailTemplateService;
        }

        [HttpPost]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("")]
        public async Task<IActionResult> UploadEmailTemplateAsync([FromForm]IFormFile file, [FromForm]EmailTemplateType type)
        {
            if (file?.Length == 0)
            {
                return BadRequest("Email template is missing.");
            }

            Guid clientId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value);

            using Stream templateStream = file.OpenReadStream();
            await _emailTemplateService.AddEmailTemplateAsync(templateStream, clientId, type);

            return Ok();
        }

        [HttpPatch]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("{emailTemplateId:guid}")]
        public async Task<IActionResult> UpdateEmailTemplateAsync([FromRoute] Guid emailTemplateId, [FromForm] IFormFile file)
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
        public async Task<IActionResult> GetEmailTemplateAsync([FromRoute] Guid emailTemplateId)
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

        [HttpDelete]
        [AuthorizationFilter(RoleHelper.Admin)]
        [Route("{emailTemplateId:guid}")]
        public async Task<IActionResult> DeleteEmailTemplateAsync([FromRoute] Guid emailTemplateId)
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