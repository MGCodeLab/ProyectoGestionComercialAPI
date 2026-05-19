using MediatR;

namespace Application.Features.Catalogo.ListaPrecio.ActualizarEstado;

public record ActualizarEstadoListaPrecioCommand(
    int Id,
    bool Activo
) : IRequest<int>;
