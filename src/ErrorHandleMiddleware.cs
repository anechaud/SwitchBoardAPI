using System;
using System.Net;
using Docker.DotNet;
using SwitchBoardApi.Core.Model;

namespace SwitchBoardApi
{
    public class ErrorHandleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandleMiddleware> _logger;
        public ErrorHandleMiddleware(RequestDelegate next, ILogger<ErrorHandleMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorDetails = new ErrorDetails();
            context.Response.ContentType = "application/json";
            switch (exception)
            {
                case BadHttpRequestException:
                    errorDetails.Message = exception.Message;
                    errorDetails.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case DockerContainerNotFoundException:
                    errorDetails.Message = exception.Message;
                    errorDetails.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case DockerImageNotFoundException:
                    errorDetails.Message = exception.Message;
                    errorDetails.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    errorDetails.Message = exception.Message;
                    errorDetails.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            await context.Response.WriteAsync(errorDetails.ToString());
        }
    }
}