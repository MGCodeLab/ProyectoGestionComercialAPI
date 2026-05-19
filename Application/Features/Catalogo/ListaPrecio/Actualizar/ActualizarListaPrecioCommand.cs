using MediatR;

namespace Application.Features.Catalogo.ListaPrecio.Actualizar;

public record ActualizarListaPrecioCommand(
    int Id,
    string Nombre,
    int MonedaId,
    string? Descripcion,
    bool EsDefault
) : IRequest<int>;
