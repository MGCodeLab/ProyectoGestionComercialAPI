using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Organizacion.Empresa.ActualizarEstado;

public class ActualizarEstadoEmpresaHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoEmpresaCommand, int>
{
    private readonly IEmpresaService _service;

    public ActualizarEstadoEmpresaHandler(
        IEmpresaService service,
        ILogger<ActualizarEstadoEmpresaHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarEstadoEmpresaCommand request, CancellationToken cancellationToken)
    {
        var empresa = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (empresa == null)
            throw new NotFoundException($"Empresa con Id {request.Id} no encontrada");

        empresa.Activo = request.Activo;

        await UpdateAuditableEntity(
            empresa,
            async () => await _service.Actualizar(empresa, cancellationToken),
            () => empresa.Id,
            cancellationToken
        );

        Logger.LogInformation("Empresa {Id} estado actualizado a {Activo}", request.Id, request.Activo);

        return empresa.Id;
    }
}
