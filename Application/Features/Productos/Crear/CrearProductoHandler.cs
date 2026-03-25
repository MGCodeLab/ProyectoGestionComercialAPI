using Application.Interfaces;
using AutoMapper;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Productos.Crear
{
    public class CrearProductoHandler : IRequestHandler<CrearProductoCommand, int>
    {
        private readonly IProductoService _service;
        private readonly ILogger<CrearProductoHandler> _logger;
        private readonly IMapper _mapper;
        public CrearProductoHandler(IProductoService service,
            ILogger<CrearProductoHandler> logger,
            IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(CrearProductoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creando producto {Nombre}", request.Nombre);

            var producto = _mapper.Map<Producto>(request);

            await _service.Crear(producto, cancellationToken);

            _logger.LogInformation("Producto {Nombre} creado con Id {Id}", producto.Nombre, producto.Id);

            return producto.Id;
        }
    }
}
