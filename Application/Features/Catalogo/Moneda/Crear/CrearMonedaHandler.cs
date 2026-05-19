using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.Moneda.Crear;

public class CrearMonedaHandler : IRequestHandler<CrearMonedaCommand, int>
{
    private readonly IMonedaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<CrearMonedaHandler> _logger;

    public CrearMonedaHandler(IMonedaService service, IMapper mapper, ILogger<CrearMonedaHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(CrearMonedaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando nueva moneda: {CodigoISO}", request.CodigoISO);

        var moneda = _mapper.Map<Domain.Catalogo.Moneda>(request);
        return await _service.Crear(moneda, cancellationToken);
    }
}
