using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Productos.ActualizarEstado
{
    public class ActualizarEstadoProductoHandler
        : AuditableUpdateHandlerBase<ActualizarEstadoProductoCommand, Unit>
    {
        private readonly IProductoService _service;

        public ActualizarEstadoProductoHandler(
            IProductoService service,
            ILogger<ActualizarEstadoProductoHandler> logger)
            : base(null, logger)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ActualizarEstadoProductoCommand request, CancellationToken cancellationToken)
        {
            var producto = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (producto == null)
                throw new NotFoundException("Producto no encontrado");

            producto.Activo = request.Activo;

            await UpdateAuditableEntity(
                producto,
                async () => await _service.Actualizar(cancellationToken),
                () => Unit.Value,
                cancellationToken
            );

            var accion = request.Activo ? "activado" : "inactivado";
            Logger.LogInformation("Producto {Id} {Accion} correctamente", request.Id, accion);

            return Unit.Value;
        }
    }
}
