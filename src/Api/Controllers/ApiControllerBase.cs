using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// This class provides a base for all API controllers. It includes the configuration for route and automatic model state validation. 
/// It also provides a mediator instance for handling requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator;

    /// <summary>
    /// Gets an instance of <see cref="ISender"/> for handling requests. If an instance of <see cref="ISender"/> is already available, it returns the existing instance. 
    /// Otherwise, it requests a new instance from the HTTP context's service provider.
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}