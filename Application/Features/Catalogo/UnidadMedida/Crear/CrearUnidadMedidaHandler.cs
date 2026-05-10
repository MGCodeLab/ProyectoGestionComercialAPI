using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.UnidadMedida.Crear;

public class CrearUnidadMedidaHandler : IRequestHandler<CrearUnidadMedidaCommand, int>
{
    private readonly IUnidadMedidaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<CrearUnidadMedidaHandler> _logger;

    public CrearUnidadMedidaHandler(IUnidadMedidaService service, IMapper mapper, ILogger<CrearUnidadMedidaHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(CrearUnidadMedidaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando nueva unidad de medida: {Codigo}", request.Codigo);
        var unidad = _mapper.Map<Domain.Catalogo.UnidadMedida>(request);
        return await _service.Crear(unidad, cancellationToken);
    }
}
