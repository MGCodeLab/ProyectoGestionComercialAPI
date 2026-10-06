using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.ListaPrecio.ActualizarEstado;

public class ActualizarEstadoListaPrecioHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoListaPrecioCommand, int>
{
    private readonly IListaPrecioService _service;

    public ActualizarEstadoListaPrecioHandler(
        IListaPrecioService service,
        ILogger<ActualizarEstadoListaPrecioHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarEstadoListaPrecioCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Actualizando estado de lista de precios: {Id}, Activo: {Activo}", request.Id, request.Activo);

        var lista = await _service.ObtenerPorId(request.Id, cancellationToken);
        if (lista == null)
            throw new NotFoundException($"Lista de precios con ID {request.Id} no encontrada");

        lista.Activo = request.Activo;

        await UpdateAuditableEntity(
            lista,
            async () => await _service.Actualizar(lista, cancellationToken),
            () => lista.Id,
            cancellationToken
        );

        Logger.LogInformation("Estado de lista de precios actualizado: {Id}", request.Id);

        return lista.Id;
    }
}
