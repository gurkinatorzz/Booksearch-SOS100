using System.Net;

namespace SOA100_bookLibrary.Security;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _expectedApiKey;
    private const string HeaderName = "X-LIBRARY-API-KEY";

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        
        //Hämta nyckeln från miljövariabler / app konfigurationen
        _expectedApiKey = config["LibraryApiKey"] ?? "";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        //GET är öppet
        var method = context.Request.Method;
        
        //POST, PUT, DELETE är skyddat (CRUD)
        var isWrite = 
            HttpMethods.IsPost(method) ||
            HttpMethods.IsPut(method) ||
            HttpMethods.IsPatch(method) ||
            HttpMethods.IsDelete(method);

        if (!isWrite)
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(_expectedApiKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("API-nyckel är inte konfigurerad");
            return;
        }

        //kollar headern
        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("API-nyckel saknas");
            return;
        }

        if (!string.Equals(providedKey.ToString(), _expectedApiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Fel API-nyckel");
            return;
        }
        
        await _next(context);
    }
}