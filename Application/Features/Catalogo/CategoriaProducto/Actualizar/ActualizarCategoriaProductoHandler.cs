using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.CategoriaProducto.Actualizar
{
    public class ActualizarCategoriaProductoHandler : IRequestHandler<ActualizarCategoriaProductoCommand, int>
    {
        private readonly ICategoriaProductoService _service;
        private readonly ICategoriaProductoValidatorService _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<ActualizarCategoriaProductoHandler> _logger;

        public ActualizarCategoriaProductoHandler(
            ICategoriaProductoService service,
            ICategoriaProductoValidatorService validator,
            IMapper mapper,
            ILogger<ActualizarCategoriaProductoHandler> logger)
        {
            _service = service;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(ActualizarCategoriaProductoCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ActualizarCategoriaProducto: {@request}", command);

            var categoria = await _service.ObtenerPorIdAsync(command.Id);
            if (categoria == null)
                throw new InvalidOperationException($"CategoriaProducto con ID {command.Id} no encontrada");

            // Prevenir ciclos cuando se cambia padre
            if (command.CategoriaPadreId.HasValue &&
                command.CategoriaPadreId != categoria.CategoriaPadreId)
            {
                var esDescendiente = await _validator.EsDescendienteDeAsync(command.CategoriaPadreId.Value, command.Id);
                if (esDescendiente)
                    throw new InvalidOperationException("No se puede crear ciclo: padre no puede ser descendiente");
            }

            _mapper.Map(command, categoria);
            categoria.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(categoria);
            _logger.LogInformation("ActualizarCategoriaProducto: ID {id}", categoria.Id);
            return categoria.Id;
        }
    }
}
