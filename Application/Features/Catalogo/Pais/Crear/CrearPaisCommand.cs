using MediatR;

namespace Application.Features.Catalogo.Pais.Crear;

public record CrearPaisCommand(
    string Nombre,
    string Codigo,
    string CodigoMoneda
) : IRequest<int>;
