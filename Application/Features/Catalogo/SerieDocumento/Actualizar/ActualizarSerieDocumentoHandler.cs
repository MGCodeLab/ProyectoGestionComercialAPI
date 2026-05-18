using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.Actualizar
{
    public class ActualizarSerieDocumentoHandler : IRequestHandler<ActualizarSerieDocumentoCommand, int>
    {
        private readonly ISerieDocumentoService _service;

        public ActualizarSerieDocumentoHandler(ISerieDocumentoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(ActualizarSerieDocumentoCommand command, CancellationToken cancellationToken)
        {
            var serie = await _service.ObtenerPorIdAsync(command.Id);
            if (serie == null)
                throw new InvalidOperationException($"SerieDocumento con ID {command.Id} no encontrado");

            serie.TipoComprobanteId = command.TipoComprobanteId;
            serie.SucursalId = command.SucursalId;
            serie.Serie = command.Serie;
            serie.NumeroMaximo = command.NumeroMaximo;

            await _service.Actualizar(serie);
            return serie.Id;
        }
    }
}
