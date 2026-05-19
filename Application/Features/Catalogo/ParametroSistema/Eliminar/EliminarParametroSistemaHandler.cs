using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.ParametroSistema.Eliminar;

public class EliminarParametroSistemaHandler : IRequestHandler<EliminarParametroSistemaCommand>
{
    private readonly IParametroSistemaService _service;
    private readonly ILogger<EliminarParametroSistemaHandler> _logger;

    public EliminarParametroSistemaHandler(IParametroSistemaService service, ILogger<EliminarParametroSistemaHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Handle(EliminarParametroSistemaCommand request, CancellationToken cancellationToken)
    {
        var parametro = await _service.ObtenerPorId(request.Id, false, cancellationToken);
        if (parametro == null)
            throw new NotFoundException($"Parámetro con id {request.Id} no encontrado");

        _logger.LogInformation("Eliminando parámetro: {ParametroId}", request.Id);

        await _service.Eliminar(parametro, cancellationToken);

        _logger.LogInformation("Parámetro eliminado exitosamente: {ParametroId}", request.Id);
    }
}
