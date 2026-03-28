using Application.Common.Models;
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
                _logger.LogError(ex, "Error por excepción de validación");
                await HandleValidationException(context, ex);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Recurso no encontrado");

                var response = ApiResponse<string>.Fail(ex.Message);

                await WriteResponse(context, HttpStatusCode.NotFound, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado");

                var response = ApiResponse<string>.Fail("Ocurrió un error interno");

                await WriteResponse(context, HttpStatusCode.InternalServerError, response);
            }
        }

        private static async Task HandleValidationException(HttpContext context, ValidationException ex)
        {
            var errors = ex.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = ApiResponse<List<string>>.Fail("Errores de validación", errors);

            await WriteResponse(context, HttpStatusCode.BadRequest, response);
        }

        private static async Task WriteResponse<T>(
            HttpContext context,
            HttpStatusCode statusCode,
            ApiResponse<T> response)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";
            
            response.TraceId = context.TraceIdentifier;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            await context.Response.WriteAsync(json);
        }
    }
}