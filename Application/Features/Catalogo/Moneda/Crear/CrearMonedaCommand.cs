using MediatR;

namespace Application.Features.Catalogo.Moneda.Crear;

public record CrearMonedaCommand(
    string Nombre,
    string Simbolo,
    string CodigoISO,
    bool EsMonedaBase
) : IRequest<int>;
