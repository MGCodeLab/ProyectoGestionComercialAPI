using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.ModuloSistema.ActualizarEstado;

public class ActualizarEstadoModuloSistemaHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoModuloSistemaCommand, Unit>
{
    private readonly IModuloSistemaService _service;

    public ActualizarEstadoModuloSistemaHandler(
        IModuloSistemaService service,
        ILogger<ActualizarEstadoModuloSistemaHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarEstadoModuloSistemaCommand request, CancellationToken cancellationToken)
    {
        var modulo = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (modulo == null)
            throw new NotFoundException($"Módulo con id {request.Id} no encontrado");

        Logger.LogInformation("Actualizando estado de módulo: {ModuloId} a {Activo}", request.Id, request.Activo);

        modulo.Activo = request.Activo;

        await UpdateAuditableEntity(
            modulo,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("Estado actualizado exitosamente: {ModuloId}", request.Id);

        return Unit.Value;
    }
}
