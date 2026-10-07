using Application.Handlers;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Empresa.Actualizar
{
    public class ActualizarEmpresaHandler
        : AuditableUpdateHandlerBase<ActualizarEmpresaCommand, int>
    {
        private readonly IEmpresaService _service;

        public ActualizarEmpresaHandler(
            IEmpresaService service,
            IMapper mapper,
            ILogger<ActualizarEmpresaHandler> logger)
            : base(mapper, logger)
        {
            _service = service;
        }

        public override async Task<int> Handle(ActualizarEmpresaCommand request, CancellationToken ct)
        {
            var empresa = await _service.ObtenerPorId(request.Id, true, ct);
            if (empresa == null)
                throw new KeyNotFoundException($"Empresa con Id {request.Id} no encontrada");

            Mapper.Map(request, empresa);

            await UpdateAuditableEntity(
                empresa,
                async () => await _service.Actualizar(empresa, ct),
                () => empresa.Id,
                ct
            );

            Logger.LogInformation($"Empresa actualizada: {empresa.Id}");

            return empresa.Id;
        }
    }
}
