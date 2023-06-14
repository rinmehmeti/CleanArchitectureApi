using Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Handles weather forecast related requests. The controller actions include methods for retrieving weather forecasts.
/// </summary>
public class WeatherForecastController : ApiControllerBase
{
    /// <summary>
    /// Retrieves all weather forecasts.
    /// </summary>
    /// <returns>A collection of weather forecasts.</returns>
    /// <response code="200">If the request was successful.</response>
    /// <response code="400">If parameters are invalid.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        return await Mediator.Send(new GetWeatherForecastsQuery());
    }
}