using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.Pais.Eliminar;

public class EliminarPaisHandler : IRequestHandler<EliminarPaisCommand, Unit>
{
    private readonly IPaisService _service;
    private readonly ILogger<EliminarPaisHandler> _logger;

    public EliminarPaisHandler(IPaisService service, ILogger<EliminarPaisHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<Unit> Handle(EliminarPaisCommand request, CancellationToken cancellationToken)
    {
        var pais = await _service.ObtenerPorId(request.Id, false, cancellationToken);
        if (pais == null)
            throw new NotFoundException($"País con id {request.Id} no encontrado");

        await _service.Eliminar(pais, cancellationToken);
        _logger.LogInformation("País eliminado: {PaisId}", request.Id);
        return Unit.Value;
    }
}
