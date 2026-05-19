using MediatR;

namespace Application.Features.Catalogo.TipoImpuesto.Crear
{
    public record CrearTipoImpuestoCommand(
        string Nombre,
        string Codigo,
        decimal Porcentaje,
        bool EsIncluido) : IRequest<int>;
}
