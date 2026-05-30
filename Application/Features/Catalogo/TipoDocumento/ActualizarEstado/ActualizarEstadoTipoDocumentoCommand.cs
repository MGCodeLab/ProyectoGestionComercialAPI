using MediatR;

namespace Application.Features.Catalogo.TipoDocumento.ActualizarEstado;

public record ActualizarEstadoTipoDocumentoCommand(int Id, bool Activo) : IRequest<Unit>;
