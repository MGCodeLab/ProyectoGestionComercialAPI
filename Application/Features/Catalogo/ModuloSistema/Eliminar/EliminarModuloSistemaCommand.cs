using MediatR;

namespace Application.Features.Catalogo.ModuloSistema.Eliminar;

public record EliminarModuloSistemaCommand(int Id) : IRequest;
