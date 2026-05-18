using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.TipoComprobante.Eliminar
{
    public class EliminarTipoComprobanteHandler : IRequestHandler<EliminarTipoComprobanteCommand, int>
    {
        private readonly ITipoComprobanteService _service;

        public EliminarTipoComprobanteHandler(ITipoComprobanteService service)
        {
            _service = service;
        }

        public async Task<int> Handle(EliminarTipoComprobanteCommand command, CancellationToken cancellationToken)
        {
            var tipoComprobante = await _service.ObtenerPorIdAsync(command.Id);
            if (tipoComprobante == null)
                throw new InvalidOperationException($"TipoComprobante con ID {command.Id} no encontrado");

            await _service.Eliminar(command.Id);
            return command.Id;
        }
    }
}
