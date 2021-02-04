using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Api.Models;
using BillTracker.Expenses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    [ApiController]
    [Route("dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IExpensesQuery _expensesQuery;

        public DashboardController(IExpensesQuery expensesQuery)
        {
            _expensesQuery = expensesQuery;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DashboardResponse>> Get(
            [FromQuery][Required] DateTimeOffset? fromDate,
            [FromQuery][Required] DateTimeOffset? toDate)
        {
            var expenses = await _expensesQuery.GetByUserId(this.GetUserId(), fromDate, toDate);
            if (expenses.IsError)
            {
                return BadRequest(expenses.Error);
            }

            return Ok(ModelMappings.MapExpensesToDashboardResponse(expenses.Result));
        }

        
    }
}
