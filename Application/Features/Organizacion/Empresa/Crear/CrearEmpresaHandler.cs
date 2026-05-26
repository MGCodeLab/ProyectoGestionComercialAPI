using MediatR;
using AutoMapper;
using Domain.Organizacion;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Empresa.Crear
{
    public class CrearEmpresaHandler : IRequestHandler<CrearEmpresaCommand, int>
    {
        private readonly IEmpresaService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CrearEmpresaHandler> _logger;

        public CrearEmpresaHandler(IEmpresaService service, IMapper mapper, ILogger<CrearEmpresaHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CrearEmpresaCommand request, CancellationToken ct)
        {
            // GUARD: SingleTenant
            //var empresaExistente = await _service.ObtenerPrimera();
            //if (empresaExistente != null)
            //    throw new InvalidOperationException("Solo 1 empresa permitida en sistema");

            // Map
            var empresa = _mapper.Map<Domain.Organizacion.Empresa>(request);

            // Persist
            var resultado = await _service.Crear(empresa);

            // Log
            _logger.LogInformation($"Empresa creada: {empresa.Id}");

            return resultado;
        }
    }
}
