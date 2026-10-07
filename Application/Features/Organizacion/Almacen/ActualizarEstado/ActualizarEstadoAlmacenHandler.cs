using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Organizacion.Almacen.ActualizarEstado;

public class ActualizarEstadoAlmacenHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoAlmacenCommand, int>
{
    private readonly IAlmacenService _service;

    public ActualizarEstadoAlmacenHandler(
        IAlmacenService service,
        ILogger<ActualizarEstadoAlmacenHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarEstadoAlmacenCommand request, CancellationToken cancellationToken)
    {
        var almacen = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (almacen == null)
            throw new NotFoundException($"Almacén con Id {request.Id} no encontrado");

        almacen.Activo = request.Activo;

        await UpdateAuditableEntity(
            almacen,
            async () => await _service.Actualizar(almacen, cancellationToken),
            () => almacen.Id,
            cancellationToken
        );

        Logger.LogInformation("Almacén {Id} estado actualizado a {Activo}", request.Id, request.Activo);

        return almacen.Id;
    }
}
