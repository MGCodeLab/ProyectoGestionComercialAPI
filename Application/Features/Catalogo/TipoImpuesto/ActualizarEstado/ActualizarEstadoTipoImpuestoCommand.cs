using MediatR;

namespace Application.Features.Catalogo.TipoImpuesto.ActualizarEstado
{
    public record ActualizarEstadoTipoImpuestoCommand(
        bool Activo,
        int Id = 0) : IRequest<Unit>;
}
