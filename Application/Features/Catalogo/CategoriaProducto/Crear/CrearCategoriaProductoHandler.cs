using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.CategoriaProducto.Crear
{
    public class CrearCategoriaProductoHandler : IRequestHandler<CrearCategoriaProductoCommand, int>
    {
        private readonly ICategoriaProductoService _service;
        private readonly ICategoriaProductoValidatorService _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<CrearCategoriaProductoHandler> _logger;

        public CrearCategoriaProductoHandler(
            ICategoriaProductoService service,
            ICategoriaProductoValidatorService validator,
            IMapper mapper,
            ILogger<CrearCategoriaProductoHandler> logger)
        {
            _service = service;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CrearCategoriaProductoCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CrearCategoriaProducto: {@request}", command);

            // Validar profundidad si tiene padre
            if (command.CategoriaPadreId.HasValue)
            {
                var profundidad = await _validator.CalcularProfundidadAsync(command.CategoriaPadreId.Value);
                if (profundidad >= 3)
                    throw new InvalidOperationException("Máximo 3 niveles de profundidad permitidos");
            }

            var categoria = _mapper.Map<Domain.Catalogo.CategoriaProducto>(command);
            categoria.Activo = true;

            await _service.Crear(categoria, cancellationToken);
            _logger.LogInformation("CrearCategoriaProducto: ID {id}", categoria.Id);
            return categoria.Id;
        }
    }
}
