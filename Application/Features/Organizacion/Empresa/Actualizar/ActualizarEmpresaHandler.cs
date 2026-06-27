using MediatR;
using AutoMapper;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Empresa.Actualizar
{
    public class ActualizarEmpresaHandler : IRequestHandler<ActualizarEmpresaCommand, int>
    {
        private readonly IEmpresaService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ActualizarEmpresaHandler> _logger;

        public ActualizarEmpresaHandler(IEmpresaService service, IMapper mapper, ILogger<ActualizarEmpresaHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(ActualizarEmpresaCommand request, CancellationToken ct)
        {
            var empresa = await _service.ObtenerPorId(request.Id, true);
            if (empresa == null)
                throw new KeyNotFoundException($"Empresa con Id {request.Id} no encontrada");

            _mapper.Map(request, empresa);
            empresa.FechaActualizacion = DateTime.UtcNow;
            await _service.Actualizar(empresa);

            _logger.LogInformation($"Empresa actualizada: {empresa.Id}");

            return empresa.Id;
        }
    }
}
