using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.ListaPrecio.ActualizarEstado;

public class ActualizarEstadoListaPrecioHandler : IRequestHandler<ActualizarEstadoListaPrecioCommand, int>
{
    private readonly IListaPrecioService _service;
    private readonly ILogger<ActualizarEstadoListaPrecioHandler> _logger;

    public ActualizarEstadoListaPrecioHandler(IListaPrecioService service, ILogger<ActualizarEstadoListaPrecioHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<int> Handle(ActualizarEstadoListaPrecioCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Actualizando estado de lista de precios: {Id}, Activo: {Activo}", request.Id, request.Activo);
        return await _service.ActualizarEstado(request.Id, request.Activo, cancellationToken);
    }
}
