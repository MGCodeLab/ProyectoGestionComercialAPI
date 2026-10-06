using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.ParametroSistema.Actualizar;

public class ActualizarParametroSistemaHandler
    : AuditableUpdateHandlerBase<ActualizarParametroSistemaCommand, Unit>
{
    private readonly IParametroSistemaService _service;

    public ActualizarParametroSistemaHandler(
        IParametroSistemaService service,
        IMapper mapper,
        ILogger<ActualizarParametroSistemaHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarParametroSistemaCommand request, CancellationToken cancellationToken)
    {
        var parametro = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (parametro == null)
            throw new NotFoundException($"Parámetro con id {request.Id} no encontrado");

        Logger.LogInformation("Actualizando parámetro: {ParametroId}", request.Id);

        Mapper.Map(request, parametro);

        await UpdateAuditableEntity(
            parametro,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("Parámetro actualizado exitosamente: {ParametroId}", request.Id);

        return Unit.Value;
    }
}
