using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.CondicionPago.Crear;

public class CrearCondicionPagoHandler : IRequestHandler<CrearCondicionPagoCommand, int>
{
    private readonly ICondicionPagoService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<CrearCondicionPagoHandler> _logger;

    public CrearCondicionPagoHandler(ICondicionPagoService service, IMapper mapper, ILogger<CrearCondicionPagoHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(CrearCondicionPagoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando nueva condición de pago: {Nombre}", request.Nombre);
        var condicion = _mapper.Map<Domain.Catalogo.CondicionPago>(request);
        return await _service.Crear(condicion, cancellationToken);
    }
}
