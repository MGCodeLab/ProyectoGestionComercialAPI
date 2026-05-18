using Application.Interfaces;
using Domain.Catalogo;
using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.Crear
{
    public class CrearSerieDocumentoHandler : IRequestHandler<CrearSerieDocumentoCommand, int>
    {
        private readonly ISerieDocumentoService _service;

        public CrearSerieDocumentoHandler(ISerieDocumentoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(CrearSerieDocumentoCommand command, CancellationToken cancellationToken)
        {
            var serie = new Domain.Catalogo.SerieDocumento
            {
                TipoComprobanteId = command.TipoComprobanteId,
                SucursalId = command.SucursalId,
                Serie = command.Serie,
                NumeroActual = 0,
                NumeroMaximo = command.NumeroMaximo,
                Activo = true
            };

            await _service.Crear(serie);
            return serie.Id;
        }
    }
}
