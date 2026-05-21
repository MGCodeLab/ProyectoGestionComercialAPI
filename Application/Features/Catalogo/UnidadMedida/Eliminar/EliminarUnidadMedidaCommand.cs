using MediatR;

namespace Application.Features.Catalogo.UnidadMedida.Eliminar;

public record EliminarUnidadMedidaCommand(int Id) : IRequest<Unit>;
