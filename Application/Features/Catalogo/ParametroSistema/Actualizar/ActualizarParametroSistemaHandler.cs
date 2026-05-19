using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.ParametroSistema.Actualizar;

public class ActualizarParametroSistemaHandler : IRequestHandler<ActualizarParametroSistemaCommand>
{
    private readonly IParametroSistemaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ActualizarParametroSistemaHandler> _logger;

    public ActualizarParametroSistemaHandler(IParametroSistemaService service, IMapper mapper, ILogger<ActualizarParametroSistemaHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Handle(ActualizarParametroSistemaCommand request, CancellationToken cancellationToken)
    {
        var parametro = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (parametro == null)
            throw new NotFoundException($"Parámetro con id {request.Id} no encontrado");

        _logger.LogInformation("Actualizando parámetro: {ParametroId}", request.Id);

        _mapper.Map(request, parametro);
        parametro.FechaActualizacion = DateTime.UtcNow;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("Parámetro actualizado exitosamente: {ParametroId}", request.Id);
    }
}
