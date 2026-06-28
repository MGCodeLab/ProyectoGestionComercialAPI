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

    public async Task<int> Handle(ActualizarProveedorCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Actualizando proveedor: {Id}", request.Id);
        var proveedor = Mapper.Map<Domain.Comercial.Proveedor>(request);

        return await UpdateAuditableEntity(
            proveedor,
            async () => await _service.Actualizar(proveedor, cancellationToken),
            () => proveedor.Id,
            cancellationToken
        );
    }
}
