using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BillTracker.Api.Models.Identity;
using BillTracker.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    [Route("identity")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [AllowAnonymous]
        [HttpPost("user/register")]
        [ProducesResponseType(typeof(LoginRequest), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _identityService.Register(
                emailAddress: request.EmailAddress,
                password: request.Password,
                firstName: request.FirstName,
                lastName: request.LastName);

            return result.Match<ActionResult>(
                success => CreatedAtAction(nameof(Login), new LoginRequest
                {
                    EmailAddress = request.EmailAddress,
                    Password = request.Password
                }),
                error => BadRequest(error));
        }

        [AllowAnonymous]
        [HttpPost("user/login")]
        [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _identityService.Login(request.EmailAddress, request.Password);

            return result.Match<ActionResult>(
                success => Ok(success),
                error => Unauthorized(error));
        }

        [HttpPost("token/revoke")]
        [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            var result = await _identityService.RevokeToken(request.Token);

            return result.Match<ActionResult>(
                success => NoContent(),
                error => Unauthorized(error));
        }

        [AllowAnonymous]
        [HttpPost("token/refresh")]
        [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _identityService.RefreshToken(request.Token);

            return result.Match<ActionResult>(
                success => Ok(success),
                error => Unauthorized(error));
        }
    }
}
