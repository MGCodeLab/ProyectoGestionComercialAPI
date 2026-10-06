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

        if (request.EsDefault)
        {
            var listaDefaultActual = await _service.ObtenerDefaultAsync(cancellationToken);
            if (listaDefaultActual != null && listaDefaultActual.Id != request.Id)
            {
                listaDefaultActual.EsDefault = false;
                await _service.Actualizar(listaDefaultActual, cancellationToken);
            }
        }

        var lista = Mapper.Map<Domain.Catalogo.ListaPrecio>(request);

        return await UpdateAuditableEntity(
            lista,
            async () => await _service.Actualizar(lista, cancellationToken),
            () => lista.Id,
            cancellationToken
        );
    }
}
