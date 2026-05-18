using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.Eliminar
{
    public class EliminarSerieDocumentoHandler : IRequestHandler<EliminarSerieDocumentoCommand, int>
    {
        private readonly ISerieDocumentoService _service;

        public EliminarSerieDocumentoHandler(ISerieDocumentoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(EliminarSerieDocumentoCommand command, CancellationToken cancellationToken)
        {
            var serie = await _service.ObtenerPorIdAsync(command.Id);
            if (serie == null)
                throw new InvalidOperationException($"SerieDocumento con ID {command.Id} no encontrado");

            await _service.Eliminar(command.Id);
            return command.Id;
        }
    }
}
