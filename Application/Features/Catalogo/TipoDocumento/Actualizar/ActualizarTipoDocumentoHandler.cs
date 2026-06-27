using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoDocumento.Actualizar;

public class ActualizarTipoDocumentoHandler : IRequestHandler<ActualizarTipoDocumentoCommand, Unit>
{
    private readonly ITipoDocumentoService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ActualizarTipoDocumentoHandler> _logger;

    public ActualizarTipoDocumentoHandler(ITipoDocumentoService service, IMapper mapper, ILogger<ActualizarTipoDocumentoHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Unit> Handle(ActualizarTipoDocumentoCommand request, CancellationToken cancellationToken)
    {
        var tipoDocumento = await _service.ObtenerPorId(request.Id, false, cancellationToken);
        if (tipoDocumento == null)
            throw new NotFoundException($"Tipo de documento con id {request.Id} no encontrado");

        _mapper.Map(request, tipoDocumento);
        tipoDocumento.FechaActualizacion = DateTime.UtcNow;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("Tipo de documento {Id} actualizado", request.Id);
        return Unit.Value;
    }
}
