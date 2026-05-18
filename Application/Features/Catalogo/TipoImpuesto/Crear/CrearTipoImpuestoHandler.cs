using Application.Interfaces;
using Domain.Catalogo;
using MediatR;

namespace Application.Features.Catalogo.TipoImpuesto.Crear
{
    public class CrearTipoImpuestoHandler : IRequestHandler<CrearTipoImpuestoCommand, int>
    {
        private readonly ITipoImpuestoService _service;

        public CrearTipoImpuestoHandler(ITipoImpuestoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(CrearTipoImpuestoCommand command, CancellationToken cancellationToken)
        {
            var tipoImpuesto = new Domain.Catalogo.TipoImpuesto
            {
                Nombre = command.Nombre,
                Codigo = command.Codigo,
                Porcentaje = command.Porcentaje,
                EsIncluido = command.EsIncluido,
                Activo = true
            };

            await _service.Crear(tipoImpuesto);
            return tipoImpuesto.Id;
        }
    }
}
