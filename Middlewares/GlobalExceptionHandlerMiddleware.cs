namespace Pronia.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // request handler

            try
            {
                await _next.Invoke(context);

            }

            catch (Exception ex)
            {
                string path = Path.Combine("http://localhost:5270", "exception", "index");
                Console.WriteLine(path);
                context.Response.Redirect($"{path}?exMessage={ex.Message}");
            }



        }
       
    }
}
