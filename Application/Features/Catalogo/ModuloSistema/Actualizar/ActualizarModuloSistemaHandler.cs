using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.ModuloSistema.Actualizar;

public class ActualizarModuloSistemaHandler : IRequestHandler<ActualizarModuloSistemaCommand>
{
    private readonly IModuloSistemaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<ActualizarModuloSistemaHandler> _logger;

    public ActualizarModuloSistemaHandler(IModuloSistemaService service, IMapper mapper, ILogger<ActualizarModuloSistemaHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Handle(ActualizarModuloSistemaCommand request, CancellationToken cancellationToken)
    {
        var modulo = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (modulo == null)
            throw new NotFoundException($"Módulo con id {request.Id} no encontrado");

        _logger.LogInformation("Actualizando módulo: {ModuloId}", request.Id);

        _mapper.Map(request, modulo);
        modulo.FechaActualizacion = DateTime.UtcNow;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("Módulo actualizado exitosamente: {ModuloId}", request.Id);
    }
}
