using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IdealAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> logger;
        private readonly RequestDelegate next;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger,RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = new Guid();

                logger.LogError($"{errorId} : {ex.Message}");
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType= "application/json";


                var error = new
                {
                    Id = errorId,
                    ErrorMesdage = "Something went wrong!"
                };

                await httpContext.Response.WriteAsJsonAsync(error);
            }
        }
    }
}
