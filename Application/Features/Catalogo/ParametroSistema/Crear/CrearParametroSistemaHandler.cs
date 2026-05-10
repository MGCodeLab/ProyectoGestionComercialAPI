using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.ParametroSistema.Crear;

public class CrearParametroSistemaHandler : IRequestHandler<CrearParametroSistemaCommand, int>
{
    private readonly IParametroSistemaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<CrearParametroSistemaHandler> _logger;

    public CrearParametroSistemaHandler(IParametroSistemaService service, IMapper mapper, ILogger<CrearParametroSistemaHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(CrearParametroSistemaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando parámetro: {Clave}", request.Clave);
        var parametro = _mapper.Map<Domain.Configuracion.ParametroSistema>(request);
        return await _service.Crear(parametro, cancellationToken);
    }
}
