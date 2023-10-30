using Microsoft.AspNetCore.Mvc;
using MultiTenantOpenProject.API.Extensions;
using MultiTenantOpenProject.API.Models;
using MultiTenantOpenProject.API.Tenancy.Models;
using MultiTenantOpenProject.API.Tenancy.Services.Interfaces;
using MultiTenantOpenProject.Contracts.Tenancy;

namespace MultiTenantOpenProject.API.Account.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = "v1")]
    public class TenancyController : ControllerBase
    {
        private readonly ILogger<TenancyController> _logger;
        private readonly ITenantService _tenantService;

        public TenancyController(ILogger<TenancyController> logger, ITenantService tenantService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        }

        /// <summary>
        /// Returns the Tenant with the given Id if the user belongs to that Tenant
        /// </summary>
        /// <remarks>*Requires Authorization</remarks>
        /// <returns>Tenant By Id</returns>
        /// <response code="200">Request completed successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized Request</response>
        /// <response code="500">Internal error has occurred</response>
        public async Task<ActionResult<TenantContract>> Get(Guid tenantId)
        {
            return Ok(await _tenantService.GetAsync(tenantId, HttpContext.GetUserId()));
        }


        /// <summary>
        /// Registers a Tenant
        /// </summary>
        /// <param name="registerContract">The required contract for registration</param>
        /// <returns>
        /// For a successful registration returns 201
        /// </returns>
        /// <response code="201">Request completed Successfully</response>
        /// <response code="400">Incorrect payload or malformed get url</response>
        /// <response code="422">Could not process Entity</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateContract), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApplicationProblemDetailsModel), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<TenantContract>> Create(CreateContract contract)
        {
            TenantModel createdTenant = await _tenantService.CreateAsync(contract.Identifier);

            string? getUrl = Url.Action(nameof(Get) ?? string.Empty, new { createdTenant.Id });

            if (getUrl is null)
            {
                return BadRequest();
            }

            return Created(getUrl, createdTenant);
        }
    }
}
