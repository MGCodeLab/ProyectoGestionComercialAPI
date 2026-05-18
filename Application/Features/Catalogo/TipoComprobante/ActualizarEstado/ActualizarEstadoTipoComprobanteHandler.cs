using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.TipoComprobante.ActualizarEstado
{
    public class ActualizarEstadoTipoComprobanteHandler : IRequestHandler<ActualizarEstadoTipoComprobanteCommand, int>
    {
        private readonly ITipoComprobanteService _service;

        public ActualizarEstadoTipoComprobanteHandler(ITipoComprobanteService service)
        {
            _service = service;
        }

        public async Task<int> Handle(ActualizarEstadoTipoComprobanteCommand command, CancellationToken cancellationToken)
        {
            var tipoComprobante = await _service.ObtenerPorIdAsync(command.Id);
            if (tipoComprobante == null)
                throw new InvalidOperationException($"TipoComprobante con ID {command.Id} no encontrado");

            tipoComprobante.Activo = command.Activo;

            await _service.Actualizar(tipoComprobante);
            return tipoComprobante.Id;
        }
    }
}
