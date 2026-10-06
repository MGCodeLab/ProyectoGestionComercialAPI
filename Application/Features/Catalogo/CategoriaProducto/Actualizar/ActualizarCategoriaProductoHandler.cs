using Application.Handlers;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.CategoriaProducto.Actualizar
{
    public class ActualizarCategoriaProductoHandler
        : AuditableUpdateHandlerBase<ActualizarCategoriaProductoCommand, int>
    {
        private readonly ICategoriaProductoService _service;
        private readonly ICategoriaProductoValidatorService _validator;

        public ActualizarCategoriaProductoHandler(
            ICategoriaProductoService service,
            ICategoriaProductoValidatorService validator,
            IMapper mapper,
            ILogger<ActualizarCategoriaProductoHandler> logger)
            : base(mapper, logger)
        {
            _service = service;
            _validator = validator;
        }

        public override async Task<int> Handle(ActualizarCategoriaProductoCommand command, CancellationToken cancellationToken)
        {
            Logger.LogInformation("ActualizarCategoriaProducto: {@request}", command);

            var categoria = await _service.ObtenerPorId(command.Id, tracking: true, cancellationToken);
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

            Mapper.Map(command, categoria);

            // Usar el método base que establece FechaActualizacion automáticamente
            return await UpdateAuditableEntity(
                categoria,
                async () => await _service.Actualizar(categoria, cancellationToken),
                () => categoria.Id,
                cancellationToken
            );
        }
    }
}
