using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.TipoImpuesto.Eliminar
{
    public class EliminarTipoImpuestoHandler : IRequestHandler<EliminarTipoImpuestoCommand, int>
    {
        private readonly ITipoImpuestoService _service;

        public EliminarTipoImpuestoHandler(ITipoImpuestoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(EliminarTipoImpuestoCommand command, CancellationToken cancellationToken)
        {
            var tipoImpuesto = await _service.ObtenerPorIdAsync(command.Id);
            if (tipoImpuesto == null)
                throw new InvalidOperationException($"TipoImpuesto con ID {command.Id} no encontrado");

            await _service.Eliminar(command.Id);
            return command.Id;
        }
    }
}
