using Application.Exceptions;
using Application.Handlers;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Comercial.Proveedor.Actualizar;

public class ActualizarProveedorHandler
    : AuditableUpdateHandlerBase<ActualizarProveedorCommand, int>
{
    private readonly IProveedorService _service;

    public ActualizarProveedorHandler(
        IProveedorService service,
        IMapper mapper,
        ILogger<ActualizarProveedorHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarProveedorCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Actualizando proveedor: {Id}", request.Id);

        var proveedor = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (proveedor == null)
            throw new NotFoundException($"Proveedor con id {request.Id} no encontrado");

        Mapper.Map(request, proveedor);

        return await UpdateAuditableEntity(
            proveedor,
            async () => await _service.Actualizar(proveedor, cancellationToken),
            () => proveedor.Id,
            cancellationToken
        );
    }
}
