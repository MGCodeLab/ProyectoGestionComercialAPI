using Application.Exceptions;
using Application.Features.Clientes.ActualizarEstado;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Productos.ActualizarEstado
{
    public class ActualizarEstadoProductoHandler : IRequestHandler<ActualizarEstadoProductoCommand, Unit>
    {
        private readonly IProductoService _service;
        private readonly ILogger<ActualizarEstadoProductoHandler> _logger;

        public ActualizarEstadoProductoHandler(IProductoService service,
            ILogger<ActualizarEstadoProductoHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActualizarEstadoProductoCommand request, CancellationToken cancellationToken)
        {
            var producto = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (producto == null)
                throw new NotFoundException("Producto no encontrado");

            producto.Activo = request.Activo;
            producto.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(cancellationToken);

            var accion = request.Activo ? "activado" : "inactivado";
            _logger.LogInformation("Cliente {Id} {Accion} correctamente", request.Id, accion);

            return Unit.Value;
        }
    }
}
