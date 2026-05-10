using MediatR;

namespace Application.Features.Catalogo.Pais.Eliminar;

public record EliminarPaisCommand(int Id) : IRequest<Unit>;
