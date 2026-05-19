using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Comercial.Proveedor.Actualizar;

public class ActualizarProveedorHandler : IRequestHandler<ActualizarProveedorCommand, int>
{
    private readonly IProveedorService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ActualizarProveedorHandler> _logger;

    public ActualizarProveedorHandler(IProveedorService service, IMapper mapper, ILogger<ActualizarProveedorHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(ActualizarProveedorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Actualizando proveedor: {Id}", request.Id);
        var proveedor = _mapper.Map<Domain.Comercial.Proveedor>(request);
        return await _service.Actualizar(proveedor, cancellationToken);
    }
}
