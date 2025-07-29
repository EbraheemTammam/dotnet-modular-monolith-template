using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Base.Responses;

namespace Base.Controllers;

[ApiController, Route("Api/[controller]")]
public abstract class ApiBaseController : ControllerBase
{
    protected ActionResult<TResult> HandleResult<TResult>(Response<TResult> response)
    {
        if(!response.Succeeded) return ProcessError(response.StatusCode, response.Message);
        return response.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(response.Data),
            StatusCodes.Status201Created => StatusCode(StatusCodes.Status201Created, response.Data),
            _ => StatusCode(response.StatusCode, response.Data)
        };
    }

    protected ActionResult HandleResult(Response result)
    {
        if (!result.Succeeded) return ProcessError(result.StatusCode, result.Message);
        return result.StatusCode switch
        {
            StatusCodes.Status201Created => StatusCode(StatusCodes.Status201Created),
            StatusCodes.Status204NoContent => NoContent(),
            _ => StatusCode(result.StatusCode)
        };
    }

    protected ActionResult ProcessError(int statusCode, string? message = null)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(message),
            StatusCodes.Status401Unauthorized => Unauthorized(),
            StatusCodes.Status403Forbidden => Forbid(message),
            StatusCodes.Status404NotFound => NotFound(message),
            StatusCodes.Status409Conflict => Conflict(message),
            StatusCodes.Status500InternalServerError => StatusCode(statusCode, message),
            _ => StatusCode(statusCode, message)
        };
    }
}
