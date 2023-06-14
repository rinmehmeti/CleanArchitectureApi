using Application.Common.Models;
using Application.TodoItems.Commands.CreateTodoItem;
using Application.TodoItems.Commands.DeleteTodoItem;
using Application.TodoItems.Commands.UpdateTodoItem;
using Application.TodoItems.Commands.UpdateTodoItemDetail;
using Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Handles todo items related requests. The controller actions include methods for creating, reading, updating and deleting todo items.
/// All actions in this controller require user to be authorized.
/// </summary>
[Authorize]
public class TodoItemsController : ApiControllerBase
{
    /// <summary>
    /// Retrieves paginated list of todo items based on provided query parameters.
    /// </summary>
    /// <param name="query">Query parameters for pagination and filtering.</param>
    /// <returns>Paginated list of todo items.</returns>
    /// <response code="200">Paginated list of todo items.</response>
    /// <response code="400">If the parameters are invalid</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [HttpGet]
    public async Task<ActionResult<PaginatedList<TodoItemBriefDto>>> GetTodoItemsWithPagination([FromQuery] GetTodoItemsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Creates a new todo item with the provided details.
    /// </summary>
    /// <param name="command">The command object containing todo item details.</param>
    /// <returns>The ID of the newly created todo item.</returns>
    /// <response code="200">ID of the newly created todo item.</response>
    /// <response code="400">If the parameters are invalid</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoItemCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Updates an existing todo item identified by the provided ID.
    /// </summary>
    /// <param name="id">The ID of the todo item to be updated.</param>
    /// <param name="command">The command object containing the new details of the todo item.</param>
    /// <returns>An ActionResult indicating the result of the operation.</returns>
    /// <response code="204">If update was successful.</response>
    /// <response code="400">If the parameters are invalid</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTodoItemCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Updates the details of an existing todo item identified by the provided ID.
    /// </summary>
    /// <param name="id">The ID of the todo item to be updated.</param>
    /// <param name="command">The command object containing the new details of the todo item.</param>
    /// <returns>An ActionResult indicating the result of the operation.</returns>
    /// <response code="204">If update was successful.</response>
    /// <response code="400">If the parameters are invalid</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [HttpPut("[action]")]
    public async Task<ActionResult> UpdateItemDetails(int id, UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Deletes an existing todo item identified by the provided ID.
    /// </summary>
    /// <param name="id">The ID of the todo item to be deleted.</param>
    /// <returns>An ActionResult indicating the result of the operation.</returns>
    /// <response code="204">If delete was successful.</response>
    /// <response code="400">If the parameters are invalid</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTodoItemCommand(id));

        return NoContent();
    }
}
