using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoomService.Filters;

public class ApiKeyFilter : IActionFilter
{
    private readonly string _apiKey;

    public ApiKeyFilter(IConfiguration config)
    {
        _apiKey = config["ApiKey"] ?? "";
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers
                .TryGetValue("X-API-KEY", out var key) || key != _apiKey)
        {
            context.Result = new UnauthorizedResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
