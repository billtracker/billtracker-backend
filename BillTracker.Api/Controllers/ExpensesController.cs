using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BillTracker.Api.Models.Expenses;
using BillTracker.Expenses;
using BillTracker.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    [ApiController]
    [Route("expenses")]
    public class ExpensesController : ControllerBase
    {
        private readonly IHandle<AddExpenseParameters, ResultOrError<ExpenseModel>> _addExpenseHandler;
        private readonly IExpensesQuery _expensesQuery;

        public ExpensesController(
            IHandle<AddExpenseParameters, ResultOrError<ExpenseModel>> addExpenseHandler,
            IExpensesQuery expensesQuery)
        {
            _addExpenseHandler = addExpenseHandler;
            _expensesQuery = expensesQuery;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExpenseModel>> Get(Guid id)
        {
            var result = await _expensesQuery.GetById(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ExpenseModel>>> Post(AddExpenseRequest request)
        {
            var result = await _addExpenseHandler.Handle(
                new AddExpenseParameters(this.GetUserId(), request.Name, request.Amount, request.AddedAt));

            return result.Match<ActionResult>(
                success => CreatedAtAction(nameof(Get), new { id = result.Result.Id }, result.Result),
                error => BadRequest(error));
        }
    }
}
