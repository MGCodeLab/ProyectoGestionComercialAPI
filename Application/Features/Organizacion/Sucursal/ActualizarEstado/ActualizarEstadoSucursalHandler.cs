using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Organizacion.Sucursal.ActualizarEstado;

public class ActualizarEstadoSucursalHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoSucursalCommand, int>
{
    private readonly ISucursalService _service;

    public ActualizarEstadoSucursalHandler(
        ISucursalService service,
        ILogger<ActualizarEstadoSucursalHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarEstadoSucursalCommand request, CancellationToken cancellationToken)
    {
        var sucursal = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (sucursal == null)
            throw new NotFoundException($"Sucursal con Id {request.Id} no encontrada");

        sucursal.Activo = request.Activo;

        await UpdateAuditableEntity(
            sucursal,
            async () => await _service.Actualizar(sucursal, cancellationToken),
            () => sucursal.Id,
            cancellationToken
        );

        Logger.LogInformation("Sucursal {Id} estado actualizado a {Activo}", request.Id, request.Activo);

        return sucursal.Id;
    }
}
