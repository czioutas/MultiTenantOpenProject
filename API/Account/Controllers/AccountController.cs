using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiTenantOpenProject.API.Account.Entities;
using MultiTenantOpenProject.API.Account.Models;
using MultiTenantOpenProject.API.Account.Services.Interfaces;
using MultiTenantOpenProject.API.Extensions;
using MultiTenantOpenProject.API.Models;
using MultiTenantOpenProject.Contracts;

namespace MultiTenantOpenProject.API.Account.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = "v1")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly SignInManager<ApplicationUserEntity> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(
            IAccountService accountService,
            UserManager<ApplicationUserEntity> userManager,
            SignInManager<ApplicationUserEntity> signInManager,
            ITokenService tokenService,
            ILogger<AccountController> logger)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(userManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("deactivate")]
        public async Task<IActionResult> DeactivateUser()
        {
            await _accountService.ChangeAccountActivityStatusAsync(HttpContext.GetUserId(), false);
            return Ok();
        }

        /// <summary>
        /// Registers a User
        /// </summary>
        /// <param name="registerContract">The required contract for registration</param>
        /// <returns>
        /// For a successful registration returns 201
        /// </returns>
        /// <response code="200">Request completed Successfully</response>
        /// <response code="400">Incorrect payload</response>
        /// <response code="422">Could not process Entity</response>
        [HttpPost]
        [ProducesResponseType(typeof(RegisterContract), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApplicationProblemDetailsModel), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Register(RegisterContract registerContract)
        {
            ApplicationUserModel model = new(
                password: registerContract.Password,
                email: registerContract.Email
            );

            await _accountService.RegisterAsync(model);

            return Ok();
        }

        /// <summary>
        /// Logins a user to the system
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <param name="logintoken">The user login token</param>
        /// <returns>
        /// For a successful login returns the token for the user
        /// </returns>
        /// <response code="200">Request completed Successfully</response>
        /// <response code="400">Incorrect payload</response>
        /// <response code="401">User not Authorized for such Action</response>
        /// </summary>
        [HttpGet, Route("{userId:guid}/{logintoken}", Name = "loginlink")]
        [ProducesResponseType(typeof(TokenContract), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TokenContract>> Login(Guid userId, string logintoken)
        {
            TokenModel tokens = await _accountService.LoginAsync(userId, logintoken);
            TokenContract tokenContract = new TokenContract(tokens.ShortToken, tokens.LongToken);
            return Ok(tokenContract);
        }

        /// <summary>
        /// Generates a login token for user
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <param name="logintoken">The user login token</param>
        /// <returns>
        /// For a successful login returns the login token for the user
        /// </returns>
        /// <response code="200">Request completed Successfully</response>
        /// <response code="400">Incorrect payload</response>
        /// <response code="401">User not Authorized for such Action</response>
        /// </summary>
        [HttpGet, Route("{userEmail}", Name = "GenerateLoginToken")]
        [ProducesResponseType(typeof(TokenContract), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GenerateLoginToken(string userEmail)
        {
            var data = await _accountService.GenerateLoginTokenAsync(userEmail);

            var url = Url.RouteUrl(
                "loginlink",
                values: new { userId = data.userId, logintoken = data.token },
                protocol: HttpContext.Request.Scheme,
                host: HttpContext.Request.Host.Value
            );

            return Ok(url);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            await _accountService.SignOutAsync(HttpContext.GetUserId());
            return Ok();
        }
    }
}
