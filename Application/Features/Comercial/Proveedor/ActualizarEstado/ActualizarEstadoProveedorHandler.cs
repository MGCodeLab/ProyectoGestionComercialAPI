using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces;

namespace Application.Features.Comercial.Proveedor.ActualizarEstado;

public class ActualizarEstadoProveedorHandler : IRequestHandler<ActualizarEstadoProveedorCommand, int>
{
    private readonly IProveedorService _service;
    private readonly ILogger<ActualizarEstadoProveedorHandler> _logger;

    public ActualizarEstadoProveedorHandler(IProveedorService service, ILogger<ActualizarEstadoProveedorHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<int> Handle(ActualizarEstadoProveedorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Actualizando estado de proveedor: {Id}, Activo: {Activo}", request.Id, request.Activo);
        return await _service.ActualizarEstado(request.Id, request.Activo, cancellationToken);
    }
}
