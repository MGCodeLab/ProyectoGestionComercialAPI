using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoDocumento.Eliminar;

public class EliminarTipoDocumentoHandler : IRequestHandler<EliminarTipoDocumentoCommand, Unit>
{
    private readonly ITipoDocumentoService _service;
    private readonly ILogger<EliminarTipoDocumentoHandler> _logger;

    public EliminarTipoDocumentoHandler(ITipoDocumentoService service, ILogger<EliminarTipoDocumentoHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<Unit> Handle(EliminarTipoDocumentoCommand request, CancellationToken cancellationToken)
    {
        var tipoDocumento = await _service.ObtenerPorId(request.Id, false, cancellationToken);
        if (tipoDocumento == null)
            throw new NotFoundException($"Tipo de documento con id {request.Id} no encontrado");

        var tieneDependencias = await _service.TieneDependencias(tipoDocumento, cancellationToken);
        if (tieneDependencias)
            throw new BadRequestException("Tipo de documento en uso en empresas, proveedores o series de documento. Solo se permite deshabilitar mediante PATCH /inactivar");

        await _service.Eliminar(tipoDocumento, cancellationToken);

        _logger.LogInformation("Tipo de documento {Id} eliminado", request.Id);
        return Unit.Value;
    }
}
