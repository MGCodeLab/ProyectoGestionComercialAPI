using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.TipoComprobante.Actualizar
{
    public class ActualizarTipoComprobanteHandler : IRequestHandler<ActualizarTipoComprobanteCommand, int>
    {
        private readonly ITipoComprobanteService _service;

        public ActualizarTipoComprobanteHandler(ITipoComprobanteService service)
        {
            _service = service;
        }

        public async Task<int> Handle(ActualizarTipoComprobanteCommand command, CancellationToken cancellationToken)
        {
            var tipoComprobante = await _service.ObtenerPorIdAsync(command.Id);
            if (tipoComprobante == null)
                throw new InvalidOperationException($"TipoComprobante con ID {command.Id} no encontrado");

            tipoComprobante.Nombre = command.Nombre;
            tipoComprobante.Codigo = command.Codigo;
            tipoComprobante.AfectaInventario = command.AfectaInventario;
            tipoComprobante.AfectaContable = command.AfectaContable;

            await _service.Actualizar(tipoComprobante);
            return tipoComprobante.Id;
        }
    }
}
