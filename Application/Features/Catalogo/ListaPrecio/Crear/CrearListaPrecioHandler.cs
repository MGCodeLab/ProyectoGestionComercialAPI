using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.ListaPrecio.Crear;

public class CrearListaPrecioHandler : IRequestHandler<CrearListaPrecioCommand, int>
{
    private readonly IListaPrecioService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<CrearListaPrecioHandler> _logger;

    public CrearListaPrecioHandler(IListaPrecioService service, IMapper mapper, ILogger<CrearListaPrecioHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(CrearListaPrecioCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando nueva lista de precios: {Nombre}", request.Nombre);

        if (request.EsDefault)
        {
            var listaDefaultActual = await _service.ObtenerDefaultAsync(cancellationToken);
            if (listaDefaultActual != null)
            {
                listaDefaultActual.EsDefault = false;
                await _service.Actualizar(listaDefaultActual, cancellationToken);
            }
        }

        var lista = _mapper.Map<Domain.Catalogo.ListaPrecio>(request);
        return await _service.Crear(lista, cancellationToken);
    }
}
