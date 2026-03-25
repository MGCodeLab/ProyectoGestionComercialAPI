using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Productos.ActualizarEstado
{
    public class ActualizarEstadoProductoHandler : IRequestHandler<ActualizarEstadoProductoCommand, Unit>
    {
        private readonly IProductoService _service;
        
        public ActualizarEstadoProductoHandler(IProductoService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ActualizarEstadoProductoCommand request, CancellationToken cancellationToken)
        {
            var producto = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (producto == null)
                throw new NotFoundException("Producto no encontrado");

            producto.Activo = request.Activo;

            await _service.Actualizar(cancellationToken);

            return Unit.Value;
        }
    }
}
