using MediatR;

namespace Application.Features.Catalogo.CondicionPago.Crear;

public record CrearCondicionPagoCommand(
    string Nombre,
    int DiasCredito,
    string? Descripcion
) : IRequest<int>;
