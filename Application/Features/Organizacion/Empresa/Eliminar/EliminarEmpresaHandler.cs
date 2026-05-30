using Application.Exceptions;
using Application.Interfaces;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Empresa.Eliminar
{
    public class EliminarEmpresaHandler : IRequestHandler<EliminarEmpresaCommand, int>
    {
        private readonly IEmpresaService _service;
        private readonly ILogger<EliminarEmpresaHandler> _logger;

        public EliminarEmpresaHandler(IEmpresaService service, ILogger<EliminarEmpresaHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(EliminarEmpresaCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _service.ObtenerPorId(request.Id, true);
            if (empresa == null)
                throw new KeyNotFoundException($"Empresa con Id {request.Id} no encontrada");

            var tieneDependencias = await _service.TieneDependencias(empresa, cancellationToken);
            if (tieneDependencias)
                throw new BadRequestException("Empresa en uso en sucursal. Solo se permite deshabilitar mediante PATCH /inactivar");

            await _service.Eliminar(request.Id);

            _logger.LogInformation($"Empresa eliminada: {request.Id}");

            return request.Id;
        }
    }
}
