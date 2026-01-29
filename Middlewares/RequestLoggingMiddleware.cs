namespace MiddlewareFilterDI.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("Middleware: Request started");

            // Call the next middleware in pipeline
            await _next(context);

            Console.WriteLine("Middleware: Request finished");
        }
    }
}
