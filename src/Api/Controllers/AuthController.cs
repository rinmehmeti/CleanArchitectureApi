using Application.Users.Commands.AuthenticateUser;
using Application.Users.Commands.CreateUser;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// This class represents a controller that handles user authentication related requests.
/// </summary>
public class AuthController : ApiControllerBase
{
    /// <summary>
    /// Registers a new user with the provided details.
    /// </summary>
    /// <param name="command">The command object containing user registration details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ActionResult with a string indicating the registration result.</returns>
    /// <response code="200">Returns the ID of the newly created user</response>
    /// <response code="400">If the parameters are invalid</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [HttpPost("[action]")]
    public async Task<ActionResult<string>> Register(RegisterCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Logs in a user with the provided login details.
    /// </summary>
    /// <param name="command">The command object containing user login details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ActionResult with a LoginResponse object containing the login result and any additional data.</returns>
    /// <response code="200">Returns an object containing user deatails and credentials</response>
    /// <response code="400">If the parameters are invalid</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [HttpPost("[action]")]
    public async Task<ActionResult<LoginResponse>> Login(LoginCommand command)
    {
        return await Mediator.Send(command);
    }
}