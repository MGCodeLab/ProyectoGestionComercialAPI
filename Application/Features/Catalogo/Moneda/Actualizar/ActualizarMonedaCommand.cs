using MediatR;

namespace Application.Features.Catalogo.Moneda.Actualizar;

public record ActualizarMonedaCommand(
    int Id,
    string Nombre,
    string Simbolo,
    string CodigoISO,
    bool EsMonedaBase
) : IRequest<Unit>;
