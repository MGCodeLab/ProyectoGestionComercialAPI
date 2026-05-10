using MediatR;

namespace Application.Features.Catalogo.Moneda.ActualizarEstado;

public record ActualizarEstadoMonedaCommand(int Id, bool Activo) : IRequest<Unit>;
