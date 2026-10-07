using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{
    /// <summary>
    /// Base class para handlers que actualizan entidades AuditableEntity.
    ///
    /// Garantiza que FechaActualizacion se establezca automáticamente
    /// en TODOS los updates sin riesgo de olvido.
    ///
    /// Patrón de uso en handler:
    /// 1. Obtener entidad con tracking=true
    /// 2. Mapear cambios
    /// 3. Llamar a UpdateAuditableEntity() que:
    ///    - Establece FechaActualizacion = DateTime.UtcNow
    ///    - Llama al servicio para guardar
    ///    - Retorna la respuesta
    /// </summary>
    public abstract class AuditableUpdateHandlerBase<TCommand, TResponse>
        : IRequestHandler<TCommand, TResponse>
        where TCommand : IRequest<TResponse>
    {
        protected readonly IMapper Mapper;
        protected readonly ILogger Logger;

        protected AuditableUpdateHandlerBase(IMapper mapper, ILogger logger)
        {
            Mapper = mapper;
            Logger = logger;
        }

        public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);

        /// <summary>
        /// Ejecuta la actualización de una entidad auditable con timestamp automático.
        ///
        /// Patrón:
        /// 1. Establece FechaActualizacion = DateTime.UtcNow
        /// 2. Ejecuta la acción de persistencia (saveChanges)
        /// 3. Retorna la respuesta
        ///
        /// Ejemplo:
        /// await UpdateAuditableEntity(
        ///     categoria,
        ///     async () => await _service.Actualizar(categoria, cancellationToken),
        ///     () => categoria.Id,
        ///     cancellationToken
        /// );
        /// </summary>
        protected async Task<TResponse> UpdateAuditableEntity<TEntity>(
            TEntity entity,
            Func<Task> persistenceAction,
            Func<TResponse> createResponse,
            CancellationToken cancellationToken)
            where TEntity : AuditableEntity
        {
            // Establecer timestamp automáticamente
            entity.FechaActualizacion = DateTime.UtcNow;

            // Ejecutar la persistencia
            await persistenceAction();

            // Retornar respuesta
            return createResponse();
        }
    }
}
