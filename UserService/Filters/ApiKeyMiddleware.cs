namespace UserService.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
    
        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
    
        public async Task InvokeAsync(HttpContext context)
        {
            // Skip API key validation for certain endpoints
            var path = context.Request.Path.Value?.ToLower();
            if (IsPublicEndpoint(path))
            {
                await _next(context);
                return;
            }

            // ✅ FIXED: Get from Environment Variables first since that's how it's configured
            var expectedApiKey = Environment.GetEnvironmentVariable("UserApiKey") ??           // ✅ Check env var first
                               _configuration["UserApiKey"] ?? 
                               _configuration["UserService:ApiKey"] ?? 
                               Environment.GetEnvironmentVariable("USER_SERVICE_API_KEY") ?? "";

            if (string.IsNullOrEmpty(expectedApiKey))
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("API Key not configured in Azure");
                return;
            }
        
            // Check for API key in header
            if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKeyHeader))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is required");
                return;
            }
        
            var providedApiKey = apiKeyHeader.FirstOrDefault();
            if (string.IsNullOrEmpty(providedApiKey) || providedApiKey != expectedApiKey)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }
        
            // API key is valid, continue to next middleware
            await _next(context);
        }
    
        private static bool IsPublicEndpoint(string? path)
        {
            if (string.IsNullOrEmpty(path)) return false;
        
            var publicPaths = new[]
            {
                "/api/users/health",  // Allow health check without API key
                "/openapi",
                "/swagger"
            };
        
            return publicPaths.Any(publicPath => path.Contains(publicPath));
        }
    }
}