using Application.Exceptions;
using Application.Handlers;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.ListaPrecio.Actualizar;

public class ActualizarListaPrecioHandler
    : AuditableUpdateHandlerBase<ActualizarListaPrecioCommand, int>
{
    private readonly IListaPrecioService _service;

    public ActualizarListaPrecioHandler(
        IListaPrecioService service,
        IMapper mapper,
        ILogger<ActualizarListaPrecioHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarListaPrecioCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Actualizando lista de precios: {Id}", request.Id);

        var lista = await _service.ObtenerPorId(request.Id, cancellationToken);
        if (lista == null)
            throw new NotFoundException($"Lista de precios con id {request.Id} no encontrada");

        if (request.EsDefault)
        {
            // Ambas entidades quedan trackeadas: la baja del default anterior se persiste
            // en el mismo SaveChanges que la actualización (atómico).
            var listaDefaultActual = await _service.ObtenerDefaultAsync(cancellationToken);
            if (listaDefaultActual != null && listaDefaultActual.Id != request.Id)
            {
                listaDefaultActual.EsDefault = false;
                listaDefaultActual.FechaActualizacion = DateTime.UtcNow;
            }
        }

        Mapper.Map(request, lista);

        return await UpdateAuditableEntity(
            lista,
            async () => await _service.Actualizar(lista, cancellationToken),
            () => lista.Id,
            cancellationToken
        );
    }
}
