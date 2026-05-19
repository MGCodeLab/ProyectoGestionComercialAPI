using MediatR;

namespace Application.Features.Catalogo.Moneda.Eliminar;

public record EliminarMonedaCommand(int Id) : IRequest<Unit>;
