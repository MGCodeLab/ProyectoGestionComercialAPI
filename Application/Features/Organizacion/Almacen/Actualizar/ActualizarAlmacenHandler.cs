using Application.Handlers;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Almacen.Actualizar
{
    public class ActualizarAlmacenHandler
        : AuditableUpdateHandlerBase<ActualizarAlmacenCommand, int>
    {
        private readonly IAlmacenService _service;

        public ActualizarAlmacenHandler(
            IAlmacenService service,
            IMapper mapper,
            ILogger<ActualizarAlmacenHandler> logger)
            : base(mapper, logger)
        {
            _service = service;
        }

        public override async Task<int> Handle(ActualizarAlmacenCommand request, CancellationToken ct)
        {
            var almacen = await _service.ObtenerPorId(request.Id, true, ct);
            if (almacen == null)
                throw new KeyNotFoundException($"Almacén con Id {request.Id} no encontrado");

            Mapper.Map(request, almacen);

            await UpdateAuditableEntity(
                almacen,
                async () => await _service.Actualizar(almacen, ct),
                () => almacen.Id,
                ct
            );

            Logger.LogInformation($"Almacén actualizado: {almacen.Id}");

            return almacen.Id;
        }
    }
}
