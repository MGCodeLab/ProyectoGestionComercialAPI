using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.CondicionPago.Actualizar;

public class ActualizarCondicionPagoHandler : IRequestHandler<ActualizarCondicionPagoCommand, int>
{
    private readonly ICondicionPagoService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ActualizarCondicionPagoHandler> _logger;

    public ActualizarCondicionPagoHandler(ICondicionPagoService service, IMapper mapper, ILogger<ActualizarCondicionPagoHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(ActualizarCondicionPagoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Actualizando condición de pago: {Id}", request.Id);
        var condicion = _mapper.Map<Domain.Catalogo.CondicionPago>(request);
        return await _service.Actualizar(condicion, cancellationToken);
    }
}
