using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.TipoImpuesto.Actualizar
{
    public class ActualizarTipoImpuestoHandler : IRequestHandler<ActualizarTipoImpuestoCommand, int>
    {
        private readonly ITipoImpuestoService _service;

        public ActualizarTipoImpuestoHandler(ITipoImpuestoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(ActualizarTipoImpuestoCommand command, CancellationToken cancellationToken)
        {
            var tipoImpuesto = await _service.ObtenerPorIdAsync(command.Id);
            if (tipoImpuesto == null)
                throw new InvalidOperationException($"TipoImpuesto con ID {command.Id} no encontrado");

            tipoImpuesto.Nombre = command.Nombre;
            tipoImpuesto.Codigo = command.Codigo;
            tipoImpuesto.Porcentaje = command.Porcentaje;
            tipoImpuesto.EsIncluido = command.EsIncluido;

            await _service.Actualizar(tipoImpuesto);
            return tipoImpuesto.Id;
        }
    }
}
