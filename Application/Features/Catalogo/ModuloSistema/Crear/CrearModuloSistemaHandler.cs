using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.ModuloSistema.Crear;

public class CrearModuloSistemaHandler : IRequestHandler<CrearModuloSistemaCommand, int>
{
    private readonly IModuloSistemaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<CrearModuloSistemaHandler> _logger;

    public CrearModuloSistemaHandler(IModuloSistemaService service, IMapper mapper, ILogger<CrearModuloSistemaHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(CrearModuloSistemaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando nuevo módulo: {Codigo}", request.Codigo);
        var modulo = _mapper.Map<Domain.Configuracion.ModuloSistema>(request);
        return await _service.Crear(modulo, cancellationToken);
    }
}
