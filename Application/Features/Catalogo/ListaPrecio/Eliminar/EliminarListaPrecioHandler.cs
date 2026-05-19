using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.ListaPrecio.Eliminar;

public class EliminarListaPrecioHandler : IRequestHandler<EliminarListaPrecioCommand, int>
{
    private readonly IListaPrecioService _service;
    private readonly ILogger<EliminarListaPrecioHandler> _logger;

    public EliminarListaPrecioHandler(IListaPrecioService service, ILogger<EliminarListaPrecioHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<int> Handle(EliminarListaPrecioCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Eliminando lista de precios: {Id}", request.Id);
        return await _service.Eliminar(request.Id, cancellationToken);
    }
}
