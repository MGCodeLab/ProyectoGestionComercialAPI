using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.ParametroSistema.ActualizarEstado;

public class ActualizarEstadoParametroSistemaHandler : IRequestHandler<ActualizarEstadoParametroSistemaCommand>
{
    private readonly IParametroSistemaService _service;
    private readonly ILogger<ActualizarEstadoParametroSistemaHandler> _logger;

    public ActualizarEstadoParametroSistemaHandler(IParametroSistemaService service, ILogger<ActualizarEstadoParametroSistemaHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Handle(ActualizarEstadoParametroSistemaCommand request, CancellationToken cancellationToken)
    {
        var parametro = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (parametro == null)
            throw new NotFoundException($"Parámetro con id {request.Id} no encontrado");

        _logger.LogInformation("Actualizando estado de parámetro: {ParametroId} a {Activo}", request.Id, request.Activo);

        parametro.Activo = request.Activo;
        parametro.FechaActualizacion = DateTime.UtcNow;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("Estado actualizado exitosamente: {ParametroId}", request.Id);
    }
}
