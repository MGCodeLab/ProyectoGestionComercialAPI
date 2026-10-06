using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.MarcaProducto.ActualizarEstado;

public class ActualizarEstadoMarcaProductoHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoMarcaProductoCommand, int>
{
    private readonly IMarcaProductoService _service;

    public ActualizarEstadoMarcaProductoHandler(
        IMarcaProductoService service,
        ILogger<ActualizarEstadoMarcaProductoHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarEstadoMarcaProductoCommand command, CancellationToken cancellationToken)
    {
        Logger.LogInformation("ActualizarEstadoMarcaProducto: {@request}", command);

        var marca = await _service.ObtenerPorId(command.Id, tracking: true, cancellationToken);
        if (marca == null)
            throw new NotFoundException($"MarcaProducto con ID {command.Id} no encontrada");

        marca.Activo = command.Activo;

        await UpdateAuditableEntity(
            marca,
            async () => await _service.Actualizar(marca, cancellationToken),
            () => marca.Id,
            cancellationToken
        );

        Logger.LogInformation("ActualizarEstadoMarcaProducto: ID {id}", marca.Id);

        return marca.Id;
    }
}
