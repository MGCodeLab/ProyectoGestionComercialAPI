using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.GestionComercial.Extensions
{
    public static class ControllerExtensions { 

        public static IActionResult OkResponse<T>(this ControllerBase controller, T data, string message = "") 
            => controller.Ok(ApiResponse<T>.Ok(data, message)); 

        public static IActionResult NotFoundResponse(this ControllerBase controller, string message = "Recurso no encontrado") 
            => controller.NotFound(ApiResponse<string>.Fail(message));

        public static IActionResult FailResponse(this ControllerBase controller, string message, List<string>? errors = null) 
            => controller.BadRequest(ApiResponse<string>.Fail(message, errors));

        public static IActionResult CreatedResponse<T>(this ControllerBase controller, string actionName, object routeValues,
            T data, string message = "")
        {
            return controller.CreatedAtAction(
                actionName,
                routeValues,
                ApiResponse<T>.Ok(data, message)
            );
        }
    }
}
