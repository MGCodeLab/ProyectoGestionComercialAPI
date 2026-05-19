using MediatR;

namespace Application.Features.Catalogo.UnidadMedida.ActualizarEstado;

public record ActualizarEstadoUnidadMedidaCommand(int Id, bool Activo) : IRequest;
