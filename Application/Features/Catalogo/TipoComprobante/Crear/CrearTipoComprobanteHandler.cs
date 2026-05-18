using Application.Interfaces;
using Domain.Catalogo;
using MediatR;

namespace Application.Features.Catalogo.TipoComprobante.Crear
{
    public class CrearTipoComprobanteHandler : IRequestHandler<CrearTipoComprobanteCommand, int>
    {
        private readonly ITipoComprobanteService _service;

        public CrearTipoComprobanteHandler(ITipoComprobanteService service)
        {
            _service = service;
        }

        public async Task<int> Handle(CrearTipoComprobanteCommand command, CancellationToken cancellationToken)
        {
            var tipoComprobante = new Domain.Catalogo.TipoComprobante
            {
                Nombre = command.Nombre,
                Codigo = command.Codigo,
                AfectaInventario = command.AfectaInventario,
                AfectaContable = command.AfectaContable,
                Activo = true
            };

            await _service.Crear(tipoComprobante);
            return tipoComprobante.Id;
        }
    }
}
