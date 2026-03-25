using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Productos.Eliminar.Commands
{
    public class EliminarProductoHandler : IRequestHandler<EliminarProductoCommand, Unit>
    {
        private readonly IProductoService _service;
        private readonly ILogger<EliminarProductoHandler> _logger;

        public EliminarProductoHandler(IProductoService service,
            ILogger<EliminarProductoHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<Unit> Handle(EliminarProductoCommand request, CancellationToken cancellationToken)
        {
            var producto = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (producto == null)
                throw new NotFoundException("Producto no encontrado");

            _logger.LogInformation("Elimninando producto {Id} - {Nombre}", request.Id, producto.Nombre);

            await _service.Eliminar(producto, cancellationToken);

            _logger.LogInformation("Producto Eliminado: {Id} - {Nombre}", producto.Id, producto.Nombre);

            return Unit.Value;
        }
    }
}
