using MediatR;

namespace Application.Features.Catalogo.CondicionPago.ActualizarEstado;

public record ActualizarEstadoCondicionPagoCommand(
    int Id,
    bool Activo
) : IRequest<int>;
