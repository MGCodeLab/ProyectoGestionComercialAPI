using MediatR;

namespace Application.Features.Catalogo.TipoImpuesto.Actualizar
{
    public record ActualizarTipoImpuestoCommand(
        string Nombre,
        string Codigo,
        decimal Porcentaje,
        bool EsIncluido,
        int Id = 0) : IRequest<Unit>;
}
