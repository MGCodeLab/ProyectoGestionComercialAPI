using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoDocumento.Actualizar;

public class ActualizarTipoDocumentoHandler : IRequestHandler<ActualizarTipoDocumentoCommand, Unit>
{
    private readonly ITipoDocumentoService _service;
    private readonly ILogger<ActualizarTipoDocumentoHandler> _logger;

    public ActualizarTipoDocumentoHandler(ITipoDocumentoService service, ILogger<ActualizarTipoDocumentoHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<Unit> Handle(ActualizarTipoDocumentoCommand request, CancellationToken cancellationToken)
    {
        var tipoDocumento = await _service.ObtenerPorId(request.Id, false, cancellationToken);
        if (tipoDocumento == null)
            throw new NotFoundException($"Tipo de documento con id {request.Id} no encontrado");

        tipoDocumento.Codigo = request.Codigo;
        tipoDocumento.Descripcion = request.Descripcion;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("Tipo de documento {Id} actualizado", request.Id);
        return Unit.Value;
    }
}
