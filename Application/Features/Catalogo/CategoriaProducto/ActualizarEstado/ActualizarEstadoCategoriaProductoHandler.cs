using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.CategoriaProducto.ActualizarEstado;

public class ActualizarEstadoCategoriaProductoHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoCategoriaProductoCommand, int>
{
    private readonly ICategoriaProductoService _service;

    public ActualizarEstadoCategoriaProductoHandler(
        ICategoriaProductoService service,
        ILogger<ActualizarEstadoCategoriaProductoHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarEstadoCategoriaProductoCommand command, CancellationToken cancellationToken)
    {
        Logger.LogInformation("ActualizarEstadoCategoriaProducto: {@request}", command);

        var categoria = await _service.ObtenerPorId(command.Id, tracking: true, cancellationToken);
        if (categoria == null)
            throw new NotFoundException($"CategoriaProducto con ID {command.Id} no encontrada");

        categoria.Activo = command.Activo;

        await UpdateAuditableEntity(
            categoria,
            async () => await _service.Actualizar(categoria, cancellationToken),
            () => categoria.Id,
            cancellationToken
        );

        Logger.LogInformation("ActualizarEstadoCategoriaProducto: ID {id}", categoria.Id);

        return categoria.Id;
    }
}
