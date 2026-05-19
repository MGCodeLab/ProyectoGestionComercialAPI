using MediatR;

namespace Application.Features.Catalogo.Pais.Actualizar;

public record ActualizarPaisCommand(
    int Id,
    string Nombre,
    string Codigo,
    string CodigoMoneda
) : IRequest<Unit>;
