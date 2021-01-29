using System.Threading.Tasks;
using BillTracker.Api.Models;
using BillTracker.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    [Route("user")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("register")]
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

        [HttpPost("login")]
        [ProducesResponseType(typeof(JwtTokenResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _identityService.Login(request.EmailAddress, request.Password);

            return result.Match<ActionResult>(
                success => Ok(success),
                error => BadRequest(error));
        }
    }
}
