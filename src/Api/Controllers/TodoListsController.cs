using Application.TodoLists.Commands.CreateTodoList;
using Application.TodoLists.Commands.DeleteTodoList;
using Application.TodoLists.Commands.UpdateTodoList;
using Application.TodoLists.Queries.ExportTodos;
using Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Handles todo lists related requests. The controller actions include methods for creating, reading, updating, deleting, and exporting todo lists.
/// All actions in this controller require the user to be authorized.
/// </summary>
[Authorize]
public class TodoListsController : ApiControllerBase
{
    /// <summary>
    /// Retrieves all todo lists.
    /// </summary>
    /// <returns>A view model containing the todo lists.</returns>
    /// <response code="200">If request was successful.</response>
    /// <response code="400">If parameters are invalid.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [HttpGet]
    public async Task<ActionResult<TodosVm>> Get()
    {
        return await Mediator.Send(new GetTodosQuery());
    }

    /// <summary>
    /// Exports the todo list identified by the provided ID.
    /// </summary>
    /// <param name="id">The ID of the todo list to be exported.</param>
    /// <returns>A file result containing the exported todo list.</returns>
    /// <response code="200">If request was successful.</response>
    /// <response code="400">If parameters are invalid.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("text/csv")]
    [HttpGet("{id}")]
    public async Task<FileResult> Get(int id)
    {
        var vm = await Mediator.Send(new ExportTodosQuery { ListId = id });

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    /// <summary>
    /// Creates a new todo list with the provided details.
    /// </summary>
    /// <param name="command">The command object containing the todo list details.</param>
    /// <returns>The ID of the newly created todo list.</returns>
    /// <response code="200">If request was successful.</response>
    /// <response code="400">If parameters are invalid.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoListCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Updates an existing todo list identified by the provided ID.
    /// </summary>
    /// <param name="id">The ID of the todo list to be updated.</param>
    /// <param name="command">The command object containing the new details of the todo list.</param>
    /// <returns>An ActionResult indicating the result of the operation.</returns>
    /// <response code="204">If request was successful.</response>
    /// <response code="400">If parameters are invalid.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTodoListCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Deletes an existing todo list identified by the provided ID.
    /// </summary>
    /// <param name="id">The ID of the todo list to be deleted.</param>
    /// <returns>An ActionResult indicating the result of the operation.</returns>
    /// <response code="204">If request was successful.</response>
    /// <response code="400">If parameters are invalid.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTodoListCommand(id));

        return NoContent();
    }
}
