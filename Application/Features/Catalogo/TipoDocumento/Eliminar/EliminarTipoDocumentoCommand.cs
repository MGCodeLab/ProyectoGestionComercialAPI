using MediatR;

namespace Application.Features.Catalogo.TipoDocumento.Eliminar;

public record EliminarTipoDocumentoCommand(int Id) : IRequest<Unit>;
