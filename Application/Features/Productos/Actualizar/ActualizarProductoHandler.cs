using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Productos.Actualizar
{
    public class ActualizarProductoHandler : IRequestHandler<ActualizarProductoCommand, Unit>
    {
        private readonly IProductoService _service;
        private readonly ILogger<ActualizarProductoHandler> _logger;
        private readonly IMapper _mapper;

        public ActualizarProductoHandler(IProductoService service,
            ILogger<ActualizarProductoHandler> logger,
            IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(ActualizarProductoCommand request, CancellationToken cancellationToken)
        {
            var producto = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (producto == null)
                throw new NotFoundException("Producto no encontrado");

            _logger.LogInformation("Actualizando producto {Nombre}", request.Nombre);

            _mapper.Map(request, producto);

            await _service.Actualizar(cancellationToken);

            _logger.LogInformation("Producto Actualizado: {Nombre} con Id {Id}", producto.Nombre, producto.Id);
            return Unit.Value;
        }
    }
}
