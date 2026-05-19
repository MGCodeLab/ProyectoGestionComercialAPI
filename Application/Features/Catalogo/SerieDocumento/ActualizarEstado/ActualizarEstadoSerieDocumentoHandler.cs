using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.ActualizarEstado
{
    public class ActualizarEstadoSerieDocumentoHandler : IRequestHandler<ActualizarEstadoSerieDocumentoCommand, int>
    {
        private readonly ISerieDocumentoService _service;

        public ActualizarEstadoSerieDocumentoHandler(ISerieDocumentoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(ActualizarEstadoSerieDocumentoCommand command, CancellationToken cancellationToken)
        {
            var serie = await _service.ObtenerPorIdAsync(command.Id);
            if (serie == null)
                throw new InvalidOperationException($"SerieDocumento con ID {command.Id} no encontrado");

            serie.Activo = command.Activo;

            await _service.Actualizar(serie);
            return serie.Id;
        }
    }
}
