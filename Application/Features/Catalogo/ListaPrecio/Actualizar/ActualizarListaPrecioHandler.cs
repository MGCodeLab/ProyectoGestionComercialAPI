using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.ListaPrecio.Actualizar;

public class ActualizarListaPrecioHandler : IRequestHandler<ActualizarListaPrecioCommand, int>
{
    private readonly IListaPrecioService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ActualizarListaPrecioHandler> _logger;

    public ActualizarListaPrecioHandler(IListaPrecioService service, IMapper mapper, ILogger<ActualizarListaPrecioHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(ActualizarListaPrecioCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Actualizando lista de precios: {Id}", request.Id);

        if (request.EsDefault)
        {
            var listaDefaultActual = await _service.ObtenerDefaultAsync(cancellationToken);
            if (listaDefaultActual != null && listaDefaultActual.Id != request.Id)
            {
                listaDefaultActual.EsDefault = false;
                await _service.Actualizar(listaDefaultActual, cancellationToken);
            }
        }

        var lista = _mapper.Map<Domain.Catalogo.ListaPrecio>(request);
        lista.FechaActualizacion = DateTime.UtcNow;
        return await _service.Actualizar(lista, cancellationToken);
    }
}
