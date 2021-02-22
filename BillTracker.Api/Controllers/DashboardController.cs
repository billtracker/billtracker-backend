using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BillTracker.Models;
using BillTracker.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    [ApiController]
    [Route("dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardQuery _query;

        public DashboardController(IDashboardQuery query)
        {
            _query = query;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Dashboard>> Get(
            [FromQuery] DateTimeOffset? fromDate,
            [FromQuery] DateTimeOffset? toDate)
        {
            var dashboard = await _query.GetDashboard(this.GetUserId(), fromDate, toDate);
            if (dashboard.IsError)
            {
                return BadRequest(dashboard.Error);
            }

            return Ok(dashboard.Result);
        }
    }
}
