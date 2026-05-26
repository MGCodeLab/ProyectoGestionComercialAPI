using Application.Interfaces;
using AutoMapper;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoDocumento.Crear;

public class CrearTipoDocumentoHandler : IRequestHandler<CrearTipoDocumentoCommand, int>
{
    private readonly ITipoDocumentoService _service;
    private readonly ILogger<CrearTipoDocumentoHandler> _logger;
    private readonly IMapper _mapper;

    public CrearTipoDocumentoHandler(ITipoDocumentoService service, ILogger<CrearTipoDocumentoHandler> logger, IMapper mapper)
    {
        _service = service;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(CrearTipoDocumentoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando tipo de documento {Codigo}", request.Codigo);

        var tipoDocumento = _mapper.Map<Domain.Catalogo.TipoDocumento>(request);

        await _service.Crear(tipoDocumento, cancellationToken);

        _logger.LogInformation("Tipo de documento {Codigo} creado con Id {Id}", tipoDocumento.Codigo, tipoDocumento.Id);

        return tipoDocumento.Id;
    }
}
