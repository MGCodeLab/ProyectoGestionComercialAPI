using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Comercial.Proveedor.Crear;

public class CrearProveedorHandler : IRequestHandler<CrearProveedorCommand, int>
{
    private readonly IProveedorService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<CrearProveedorHandler> _logger;

    public CrearProveedorHandler(IProveedorService service, IMapper mapper, ILogger<CrearProveedorHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(CrearProveedorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando nuevo proveedor: {RazonSocial}", request.RazonSocial);
        var proveedor = _mapper.Map<Domain.Comercial.Proveedor>(request);
        return await _service.Crear(proveedor, cancellationToken);
    }
}
