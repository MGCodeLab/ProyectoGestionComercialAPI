using Application.Handlers;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.MarcaProducto.Actualizar
{
    public class ActualizarMarcaProductoHandler
        : AuditableUpdateHandlerBase<ActualizarMarcaProductoCommand, int>
    {
        private readonly IMarcaProductoService _service;

        public ActualizarMarcaProductoHandler(
            IMarcaProductoService service,
            IMapper mapper,
            ILogger<ActualizarMarcaProductoHandler> logger)
            : base(mapper, logger)
        {
            _service = service;
        }

        public async Task<int> Handle(ActualizarMarcaProductoCommand command, CancellationToken cancellationToken)
        {
            Logger.LogInformation("ActualizarMarcaProducto: {@request}", command);

            var marca = await _service.ObtenerPorIdAsync(command.Id, tracking: true, cancellationToken);
            if (marca == null)
                throw new InvalidOperationException($"MarcaProducto con ID {command.Id} no encontrada");

            Mapper.Map(command, marca);

            // Usar el método base que establece FechaActualizacion automáticamente
            return await UpdateAuditableEntity(
                marca,
                async () => await _service.Actualizar(marca, cancellationToken),
                () => marca.Id,
                cancellationToken
            );
        }
    }
}
