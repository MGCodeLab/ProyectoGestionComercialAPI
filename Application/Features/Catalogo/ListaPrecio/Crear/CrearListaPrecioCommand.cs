using MediatR;

namespace Application.Features.Catalogo.ListaPrecio.Crear;

public record CrearListaPrecioCommand(
    string Nombre,
    int MonedaId,
    string? Descripcion,
    bool EsDefault
) : IRequest<int>;
