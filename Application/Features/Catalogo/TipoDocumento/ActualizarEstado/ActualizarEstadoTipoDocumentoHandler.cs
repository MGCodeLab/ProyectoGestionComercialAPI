using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoDocumento.ActualizarEstado;

public class ActualizarEstadoTipoDocumentoHandler : IRequestHandler<ActualizarEstadoTipoDocumentoCommand, Unit>
{
    private readonly ITipoDocumentoService _service;
    private readonly ILogger<ActualizarEstadoTipoDocumentoHandler> _logger;

    public ActualizarEstadoTipoDocumentoHandler(ITipoDocumentoService service, ILogger<ActualizarEstadoTipoDocumentoHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<Unit> Handle(ActualizarEstadoTipoDocumentoCommand request, CancellationToken cancellationToken)
    {
        var tipoDocumento = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (tipoDocumento == null)
            throw new NotFoundException($"Tipo de documento con id {request.Id} no encontrado");

        tipoDocumento.Activo = request.Activo;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("Tipo de documento {Id} estado actualizado a {Activo}", request.Id, request.Activo);
        return Unit.Value;
    }
}
