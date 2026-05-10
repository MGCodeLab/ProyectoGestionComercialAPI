using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.UnidadMedida.Actualizar;

public class ActualizarUnidadMedidaHandler : IRequestHandler<ActualizarUnidadMedidaCommand>
{
    private readonly IUnidadMedidaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ActualizarUnidadMedidaHandler> _logger;

    public ActualizarUnidadMedidaHandler(IUnidadMedidaService service, IMapper mapper, ILogger<ActualizarUnidadMedidaHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Handle(ActualizarUnidadMedidaCommand request, CancellationToken cancellationToken)
    {
        var unidad = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (unidad == null)
            throw new NotFoundException($"Unidad de medida con id {request.Id} no encontrada");

        _logger.LogInformation("Actualizando unidad de medida: {UnidadId}", request.Id);

        _mapper.Map(request, unidad);
        unidad.FechaActualizacion = DateTime.UtcNow;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("Unidad de medida actualizada exitosamente: {UnidadId}", request.Id);
    }
}
