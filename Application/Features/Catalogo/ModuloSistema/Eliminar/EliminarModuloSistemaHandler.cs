using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.ModuloSistema.Eliminar;

public class EliminarModuloSistemaHandler : IRequestHandler<EliminarModuloSistemaCommand>
{
    private readonly IModuloSistemaService _service;
    private readonly ILogger<EliminarModuloSistemaHandler> _logger;

    public EliminarModuloSistemaHandler(IModuloSistemaService service, ILogger<EliminarModuloSistemaHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Handle(EliminarModuloSistemaCommand request, CancellationToken cancellationToken)
    {
        var modulo = await _service.ObtenerPorId(request.Id, false, cancellationToken);
        if (modulo == null)
            throw new NotFoundException($"Módulo con id {request.Id} no encontrado");

        _logger.LogInformation("Eliminando módulo: {ModuloId}", request.Id);

        await _service.Eliminar(modulo, cancellationToken);

        _logger.LogInformation("Módulo eliminado exitosamente: {ModuloId}", request.Id);
    }
}
