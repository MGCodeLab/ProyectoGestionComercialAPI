using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.CondicionPago.ActualizarEstado;

public class ActualizarEstadoCondicionPagoHandler : IRequestHandler<ActualizarEstadoCondicionPagoCommand, int>
{
    private readonly ICondicionPagoService _service;
    private readonly ILogger<ActualizarEstadoCondicionPagoHandler> _logger;

    public ActualizarEstadoCondicionPagoHandler(ICondicionPagoService service, ILogger<ActualizarEstadoCondicionPagoHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<int> Handle(ActualizarEstadoCondicionPagoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Actualizando estado de condición de pago: {Id}, Activo: {Activo}", request.Id, request.Activo);
        return await _service.ActualizarEstado(request.Id, request.Activo, cancellationToken);
    }
}
