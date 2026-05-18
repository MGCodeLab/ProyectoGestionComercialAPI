using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.TipoImpuesto.ActualizarEstado
{
    public class ActualizarEstadoTipoImpuestoHandler : IRequestHandler<ActualizarEstadoTipoImpuestoCommand, int>
    {
        private readonly ITipoImpuestoService _service;

        public ActualizarEstadoTipoImpuestoHandler(ITipoImpuestoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(ActualizarEstadoTipoImpuestoCommand command, CancellationToken cancellationToken)
        {
            var tipoImpuesto = await _service.ObtenerPorIdAsync(command.Id);
            if (tipoImpuesto == null)
                throw new InvalidOperationException($"TipoImpuesto con ID {command.Id} no encontrado");

            tipoImpuesto.Activo = command.Activo;

            await _service.Actualizar(tipoImpuesto);
            return tipoImpuesto.Id;
        }
    }
}
