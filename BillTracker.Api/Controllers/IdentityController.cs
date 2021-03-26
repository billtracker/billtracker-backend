using System;
using System.Threading.Tasks;
using BillTracker.Api.Models.Identity;
using BillTracker.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    [ApiController]
    [Route("identity")]
    public class IdentityController : ControllerBase
    {
        private const string CookieRefreshToken = "refresh_token";

        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [AllowAnonymous]
        [HttpPost("user/register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register(RegisterRequest request)
        {
            var result = await _identityService.Register(
                emailAddress: request.EmailAddress,
                password: request.Password,
                userName: request.UserName);

            return result.Match<ActionResult>(
                success => Ok(),
                error => BadRequest(error));
        }

        [AllowAnonymous]
        [HttpPost("user/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthenticationResult>> Login(LoginRequest request)
        {
            var result = await _identityService.Login(request.EmailAddressOrUserName, request.Password);

            return result.Match<ActionResult>(
                success =>
                {
                    SetCookieRefreshToken(success.RefreshToken);
                    return Ok(success);
                },
                error => Unauthorized(error));
        }

        [HttpPost("token/revoke")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthenticationResult>> RevokeToken()
        {
            var token = this.Request.Cookies[CookieRefreshToken];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized($"The '{CookieRefreshToken}' cookie is empty.");
            }

            var result = await _identityService.RevokeToken(token);

            return result.Match<ActionResult>(
                success => NoContent(),
                error => Unauthorized(error));
        }

        [AllowAnonymous]
        [HttpPost("token/refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthenticationResult>> RefreshToken()
        {
            var token = this.Request.Cookies[CookieRefreshToken];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized($"The '{CookieRefreshToken}' cookie is empty.");
            }

            var result = await _identityService.RefreshToken(token);

            return result.Match<ActionResult>(
                success =>
                {
                    SetCookieRefreshToken(success.RefreshToken);
                    return Ok(success);
                },
                error => Unauthorized(error));
        }

        private void SetCookieRefreshToken(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddDays(7),
            };
            Response.Cookies.Append(CookieRefreshToken, refreshToken, cookieOptions);
        }
    }
}
