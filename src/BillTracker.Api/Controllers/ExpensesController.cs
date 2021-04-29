using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BillTracker.Api.Models;
using BillTracker.Commands;
using BillTracker.Models;
using BillTracker.Queries;
using BillTracker.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    [ApiController]
    [Route("expenses")]
    public class ExpensesController : ControllerBase
    {
        private readonly SaveExpense _saveExpenseHandler;
        private readonly ExpensesQuery _expensesQuery;

        public ExpensesController(
            SaveExpense addExpenseHandler,
            ExpensesQuery expensesQuery)
        {
            _saveExpenseHandler = addExpenseHandler;
            _expensesQuery = expensesQuery;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<ExpenseModel>>> GetMany(
            [FromQuery][Required][Range(1, int.MaxValue)] int? pageNumber,
            [FromQuery][Required][Range(5, 50)] int? pageSize,
            [FromQuery] DateTimeOffset? fromDate,
            [FromQuery] DateTimeOffset? toDate)
        {
            var result = await _expensesQuery.GetMany(
                this.GetUserId(),
                pageNumber: pageNumber.Value,
                pageSize: pageSize.Value,
                fromDate: fromDate,
                toDate: toDate);

            return result.Match<ActionResult>(
                success => Ok(success),
                error => BadRequest(error));
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

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExpenseModel>> Put(SaveExpenseRequest request)
        {
            var result = await _saveExpenseHandler.Handle(
                new SaveExpenseParameters(
                    this.GetUserId(),
                    aggregateId: request.AggregateId.Value,
                    request.Name,
                    price: request.Price,
                    amount: request.Amount,
                    expenseId: request.ExpenseId,
                    expenseTypeId: request.ExpenseTypeId));

            return result.Match<ActionResult>(
                success => result.Result is null ? NotFound() : Ok(result.Result),
                error => BadRequest(error));
        }
    }
}
