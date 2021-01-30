using System;
using System.Security.Authentication;
using System.Security.Claims;
using BillTracker.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    internal static class ControllerBaseExtensions
    {
        public static Guid GetUserId(this ControllerBase @base)
        {
            var userId = @base.User.FindFirstValue(IdentityClaims.UserId);
            if (!Guid.TryParse(userId, out var result))
            {
                throw new AuthenticationException($"Claim {IdentityClaims.UserId} is not presented");
            }

            return result;
        }
    }
}
