using System.Collections.Generic;
using System.Threading.Tasks;
using BillTracker.Api.Models;
using BillTracker.Commands;
using BillTracker.Models;
using BillTracker.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    [ApiController]
    [Route("expenses/types")]
    public class ExpenseTypesController : ControllerBase
    {
        private readonly CreateExpenseType _createExpenseType;
        private readonly ExpenseTypesQuery _expenseTypesQuery;

        public ExpenseTypesController(CreateExpenseType createExpenseType, ExpenseTypesQuery expenseTypesQuery)
        {
            _createExpenseType = createExpenseType;
            _expenseTypesQuery = expenseTypesQuery;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExpenseTypeModel>> Post(CreateExpenseTypeRequest request)
        {
            var result = await _createExpenseType.Handle(new CreateExpenseTypeParameters(this.GetUserId(), request.Name));

            return result.Match<ActionResult>(
                success => Ok(success),
                error => BadRequest(error));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IEnumerable<ExpenseTypeModel>> GetAllVisibleForUser()
        {
            return _expenseTypesQuery.GetAllVisibleForUser(this.GetUserId());
        }
    }
}
