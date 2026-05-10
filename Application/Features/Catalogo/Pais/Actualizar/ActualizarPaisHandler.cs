using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.Pais.Actualizar;

public class ActualizarPaisHandler : IRequestHandler<ActualizarPaisCommand, Unit>
{
    private readonly IPaisService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ActualizarPaisHandler> _logger;

    public ActualizarPaisHandler(IPaisService service, IMapper mapper, ILogger<ActualizarPaisHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Unit> Handle(ActualizarPaisCommand request, CancellationToken cancellationToken)
    {
        var pais = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (pais == null)
            throw new NotFoundException($"País con id {request.Id} no encontrado");

        _logger.LogInformation("Actualizando país: {PaisId}", request.Id);

        _mapper.Map(request, pais);
        pais.FechaActualizacion = DateTime.UtcNow;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("País actualizado exitosamente: {PaisId}", request.Id);

        return Unit.Value;
    }
}
