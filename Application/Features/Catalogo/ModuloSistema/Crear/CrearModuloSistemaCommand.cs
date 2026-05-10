using MediatR;

namespace Application.Features.Catalogo.ModuloSistema.Crear;

public record CrearModuloSistemaCommand(string Nombre, string Codigo, string? Descripcion) : IRequest<int>;
