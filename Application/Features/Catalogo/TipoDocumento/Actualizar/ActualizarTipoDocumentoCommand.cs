using MediatR;

namespace Application.Features.Catalogo.TipoDocumento.Actualizar;

public record ActualizarTipoDocumentoCommand(
    int Id,
    string Codigo,
    string? Descripcion
) : IRequest<Unit>;
