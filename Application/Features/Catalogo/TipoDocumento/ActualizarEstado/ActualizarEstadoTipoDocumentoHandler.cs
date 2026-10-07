using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.TipoDocumento.ActualizarEstado;

public class ActualizarEstadoTipoDocumentoHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoTipoDocumentoCommand, Unit>
{
    private readonly ITipoDocumentoService _service;

    public ActualizarEstadoTipoDocumentoHandler(
        ITipoDocumentoService service,
        ILogger<ActualizarEstadoTipoDocumentoHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarEstadoTipoDocumentoCommand request, CancellationToken cancellationToken)
    {
        var tipoDocumento = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (tipoDocumento == null)
            throw new NotFoundException($"Tipo de documento con id {request.Id} no encontrado");

        tipoDocumento.Activo = request.Activo;

        await UpdateAuditableEntity(
            tipoDocumento,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("Tipo de documento {Id} estado actualizado a {Activo}", request.Id, request.Activo);

        return Unit.Value;
    }
}
