using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.ModuloSistema.ActualizarEstado;

public class ActualizarEstadoModuloSistemaHandler : IRequestHandler<ActualizarEstadoModuloSistemaCommand>
{
    private readonly IModuloSistemaService _service;
    private readonly ILogger<ActualizarEstadoModuloSistemaHandler> _logger;

    public ActualizarEstadoModuloSistemaHandler(IModuloSistemaService service, ILogger<ActualizarEstadoModuloSistemaHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Handle(ActualizarEstadoModuloSistemaCommand request, CancellationToken cancellationToken)
    {
        var modulo = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (modulo == null)
            throw new NotFoundException($"Módulo con id {request.Id} no encontrado");

        _logger.LogInformation("Actualizando estado de módulo: {ModuloId} a {Activo}", request.Id, request.Activo);

        modulo.Activo = request.Activo;
        modulo.FechaActualizacion = DateTime.UtcNow;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("Estado actualizado exitosamente: {ModuloId}", request.Id);
    }
}
