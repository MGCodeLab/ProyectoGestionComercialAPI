using MediatR;

namespace Application.Features.Catalogo.Pais.ActualizarEstado;

public record ActualizarEstadoPaisCommand(int Id, bool Activo) : IRequest<Unit>;
