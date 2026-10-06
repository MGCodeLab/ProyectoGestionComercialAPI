using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Domain.Catalogo;
using Domain.Comercial;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Productos.Actualizar
{
    public class ActualizarProductoHandler
        : AuditableUpdateHandlerBase<ActualizarProductoCommand, Unit>
    {
        private readonly IProductoService _service;

        public ActualizarProductoHandler(
            IProductoService service,
            ILogger<ActualizarProductoHandler> logger,
            IMapper mapper)
            : base(mapper, logger)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ActualizarProductoCommand request, CancellationToken cancellationToken)
        {
            var producto = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (producto == null)
                throw new NotFoundException("Producto no encontrado");

            Logger.LogInformation("Actualizando producto {Nombre}", request.Nombre);

            Mapper.Map(request, producto);

            await UpdateAuditableEntity(
                producto,
                async () => await _service.Actualizar(cancellationToken),
                () => Unit.Value,
                cancellationToken
            );

            Logger.LogInformation("Producto Actualizado: {Nombre} con Id {Id}", producto.Nombre, producto.Id);
            return Unit.Value;
        }
    }
}
