using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.Moneda.Actualizar;

public class ActualizarMonedaHandler : IRequestHandler<ActualizarMonedaCommand, Unit>
{
    private readonly IMonedaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ActualizarMonedaHandler> _logger;

    public ActualizarMonedaHandler(IMonedaService service, IMapper mapper, ILogger<ActualizarMonedaHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Unit> Handle(ActualizarMonedaCommand request, CancellationToken cancellationToken)
    {
        var moneda = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (moneda == null)
            throw new NotFoundException($"Moneda con id {request.Id} no encontrada");

        _logger.LogInformation("Actualizando moneda: {MonedaId}", request.Id);

        _mapper.Map(request, moneda);
        moneda.FechaActualizacion = DateTime.UtcNow;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("Moneda actualizada exitosamente: {MonedaId}", request.Id);

        return Unit.Value;
    }
}
