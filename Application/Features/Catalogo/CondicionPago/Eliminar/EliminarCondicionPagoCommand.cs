using MediatR;

namespace Application.Features.Catalogo.CondicionPago.Eliminar;

public record EliminarCondicionPagoCommand(int Id) : IRequest<int>;
