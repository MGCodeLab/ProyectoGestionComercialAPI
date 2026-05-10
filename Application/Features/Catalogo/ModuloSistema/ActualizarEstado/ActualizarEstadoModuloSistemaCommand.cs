using MediatR;

namespace Application.Features.Catalogo.ModuloSistema.ActualizarEstado;

public record ActualizarEstadoModuloSistemaCommand(int Id, bool Activo) : IRequest;
