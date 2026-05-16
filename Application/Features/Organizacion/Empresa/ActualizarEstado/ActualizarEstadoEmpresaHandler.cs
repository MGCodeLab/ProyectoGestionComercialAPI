using MediatR;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Empresa.ActualizarEstado
{
    public class ActualizarEstadoEmpresaHandler : IRequestHandler<ActualizarEstadoEmpresaCommand, int>
    {
        private readonly IEmpresaService _service;
        private readonly ILogger<ActualizarEstadoEmpresaHandler> _logger;

        public ActualizarEstadoEmpresaHandler(IEmpresaService service, ILogger<ActualizarEstadoEmpresaHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(ActualizarEstadoEmpresaCommand request, CancellationToken ct)
        {
            var empresa = await _service.ObtenerPorId(request.Id, true);
            if (empresa == null)
                throw new KeyNotFoundException($"Empresa con Id {request.Id} no encontrada");

            empresa.Activo = request.Activo;
            await _service.Actualizar(empresa);

            _logger.LogInformation($"Empresa {request.Id} estado actualizado a {request.Activo}");

            return empresa.Id;
        }
    }
}
