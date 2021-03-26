using System.Threading.Tasks;
using BillTracker.Api.Models;
using BillTracker.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    [ApiController]
    [Route("expenses/aggregates")]
    public class ExpensesAggregatesController : ControllerBase
    {
        private readonly SaveExpenseAggregate _saveExpenseAggregate;

        public ExpensesAggregatesController(SaveExpenseAggregate saveExpenseAggregate)
        {
            _saveExpenseAggregate = saveExpenseAggregate;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(SaveExpenseAggregateRequest request)
        {
            var result = await _saveExpenseAggregate.Handle(
                new SaveExpenseAggregateParameters(
                    request.Id,
                    request.UserId,
                    request.Name,
                    request.AddedDate,
                    request.IsDraft));

            return result.Match<ActionResult>(
                success => Ok(success),
                error => BadRequest(error));
        }
    }
}
