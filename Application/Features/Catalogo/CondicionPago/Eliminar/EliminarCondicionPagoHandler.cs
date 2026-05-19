using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.CondicionPago.Eliminar;

public class EliminarCondicionPagoHandler : IRequestHandler<EliminarCondicionPagoCommand, int>
{
    private readonly ICondicionPagoService _service;
    private readonly ILogger<EliminarCondicionPagoHandler> _logger;

    public EliminarCondicionPagoHandler(ICondicionPagoService service, ILogger<EliminarCondicionPagoHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<int> Handle(EliminarCondicionPagoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Eliminando condición de pago: {Id}", request.Id);
        return await _service.Eliminar(request.Id, cancellationToken);
    }
}
