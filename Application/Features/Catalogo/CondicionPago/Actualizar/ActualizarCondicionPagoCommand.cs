using MediatR;

namespace Application.Features.Catalogo.CondicionPago.Actualizar;

public record ActualizarCondicionPagoCommand(
    int Id,
    string Nombre,
    int DiasCredito,
    string? Descripcion
) : IRequest<int>;
