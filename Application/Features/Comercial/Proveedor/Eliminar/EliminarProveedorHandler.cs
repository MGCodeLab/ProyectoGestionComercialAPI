using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Comercial.Proveedor.Eliminar;

public class EliminarProveedorHandler : IRequestHandler<EliminarProveedorCommand, int>
{
    private readonly IProveedorService _service;
    private readonly ILogger<EliminarProveedorHandler> _logger;

    public EliminarProveedorHandler(IProveedorService service, ILogger<EliminarProveedorHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<int> Handle(EliminarProveedorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Eliminando proveedor: {Id}", request.Id);
        return await _service.Eliminar(request.Id, cancellationToken);
    }
}
