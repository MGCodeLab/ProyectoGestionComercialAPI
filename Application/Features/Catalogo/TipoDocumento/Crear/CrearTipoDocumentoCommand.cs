using MediatR;

namespace Application.Features.Catalogo.TipoDocumento.Crear;

public record CrearTipoDocumentoCommand(
    string Codigo,
    string? Descripcion
) : IRequest<int>;
