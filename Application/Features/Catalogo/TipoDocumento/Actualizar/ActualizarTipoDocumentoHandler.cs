using Application.Exceptions;
using Application.Handlers;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoDocumento.Actualizar;

public class ActualizarTipoDocumentoHandler
    : AuditableUpdateHandlerBase<ActualizarTipoDocumentoCommand, Unit>
{
    private readonly ITipoDocumentoService _service;

    public ActualizarTipoDocumentoHandler(
        ITipoDocumentoService service,
        IMapper mapper,
        ILogger<ActualizarTipoDocumentoHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
    }

    public async Task<Unit> Handle(ActualizarTipoDocumentoCommand request, CancellationToken cancellationToken)
    {
        var tipoDocumento = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (tipoDocumento == null)
            throw new NotFoundException($"Tipo de documento con id {request.Id} no encontrado");

        Mapper.Map(request, tipoDocumento);

        return await UpdateAuditableEntity(
            tipoDocumento,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );
    }
}
