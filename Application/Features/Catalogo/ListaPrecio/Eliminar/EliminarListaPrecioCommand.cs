using MediatR;

namespace Application.Features.Catalogo.ListaPrecio.Eliminar;

public record EliminarListaPrecioCommand(int Id) : IRequest<int>;
