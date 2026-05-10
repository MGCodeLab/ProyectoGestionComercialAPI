using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Catalogo.Pais.Crear;

public class CrearPaisHandler : IRequestHandler<CrearPaisCommand, int>
{
    private readonly IPaisService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<CrearPaisHandler> _logger;

    public CrearPaisHandler(IPaisService service, IMapper mapper, ILogger<CrearPaisHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(CrearPaisCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando nuevo país: {Codigo}", request.Codigo);

        var pais = _mapper.Map<Domain.Catalogo.Pais>(request);
        return await _service.Crear(pais, cancellationToken);
    }
}
