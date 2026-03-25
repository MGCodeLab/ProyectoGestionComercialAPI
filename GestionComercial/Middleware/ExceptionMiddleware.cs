using Application.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace API.GestionComercial.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Error por exepcion de validacion");
                await HandleValidationException(context, ex);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";

                var response = JsonSerializer.Serialize(new
                {
                    error = ex.Message,
                    traceId = context.TraceIdentifier
                });

                await context.Response.WriteAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado");
                await HandleGenericException(context, ex);
            }
        }

        private static async Task HandleValidationException(HttpContext context, ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors.Select(e => e.ErrorMessage);

            var response = JsonSerializer.Serialize(new
            {
                errors,
                traceId = context.TraceIdentifier
            });

            await context.Response.WriteAsync(response);
        }

        private static async Task HandleGenericException(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(new
            {
                error = "Ocurrió un error interno",
                traceId = context.TraceIdentifier
            });

            await context.Response.WriteAsync(response);
        }
    }
}
