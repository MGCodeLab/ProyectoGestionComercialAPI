using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.MarcaProducto.Crear
{
    public class CrearMarcaProductoHandler : IRequestHandler<CrearMarcaProductoCommand, int>
    {
        private readonly IMarcaProductoService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CrearMarcaProductoHandler> _logger;

        public CrearMarcaProductoHandler(IMarcaProductoService service, IMapper mapper, ILogger<CrearMarcaProductoHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CrearMarcaProductoCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CrearMarcaProducto: {@request}", command);

            var marca = _mapper.Map<Domain.Catalogo.MarcaProducto>(command);
            marca.Activo = true;

            await _service.Crear(marca);
            _logger.LogInformation("CrearMarcaProducto: ID {id}", marca.Id);
            return marca.Id;
        }
    }
}
