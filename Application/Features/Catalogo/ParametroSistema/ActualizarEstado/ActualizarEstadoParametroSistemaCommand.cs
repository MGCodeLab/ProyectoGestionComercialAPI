using MediatR;

namespace Application.Features.Catalogo.ParametroSistema.ActualizarEstado;

public record ActualizarEstadoParametroSistemaCommand(int Id, bool Activo) : IRequest;
