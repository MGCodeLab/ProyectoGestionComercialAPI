using Application.Handlers;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Sucursal.Actualizar
{
    public class ActualizarSucursalHandler
        : AuditableUpdateHandlerBase<ActualizarSucursalCommand, int>
    {
        private readonly ISucursalService _service;

        public ActualizarSucursalHandler(
            ISucursalService service,
            IMapper mapper,
            ILogger<ActualizarSucursalHandler> logger)
            : base(mapper, logger)
        {
            _service = service;
        }

        public override async Task<int> Handle(ActualizarSucursalCommand request, CancellationToken cancellationToken)
        {
            var sucursal = await _service.ObtenerPorId(request.Id, true, cancellationToken);
            if (sucursal == null)
                throw new KeyNotFoundException($"Sucursal con Id {request.Id} no encontrada");

            Mapper.Map(request, sucursal);

            await UpdateAuditableEntity(
                sucursal,
                async () => await _service.Actualizar(sucursal, cancellationToken),
                () => sucursal.Id,
                cancellationToken
            );

            Logger.LogInformation($"Sucursal actualizada: {sucursal.Id}");

            return sucursal.Id;
        }
    }
}
