using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.ParametroSistema.ActualizarEstado;

public class ActualizarEstadoParametroSistemaHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoParametroSistemaCommand, Unit>
{
    private readonly IParametroSistemaService _service;

    public ActualizarEstadoParametroSistemaHandler(
        IParametroSistemaService service,
        ILogger<ActualizarEstadoParametroSistemaHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarEstadoParametroSistemaCommand request, CancellationToken cancellationToken)
    {
        var parametro = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (parametro == null)
            throw new NotFoundException($"Parámetro con id {request.Id} no encontrado");

        Logger.LogInformation("Actualizando estado de parámetro: {ParametroId} a {Activo}", request.Id, request.Activo);

        parametro.Activo = request.Activo;

        await UpdateAuditableEntity(
            parametro,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("Estado actualizado exitosamente: {ParametroId}", request.Id);

        return Unit.Value;
    }
}
