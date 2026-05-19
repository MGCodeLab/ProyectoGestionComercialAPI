using MediatR;

namespace Application.Features.Catalogo.ModuloSistema.Actualizar;

public record ActualizarModuloSistemaCommand(int Id, string Nombre, string Codigo, string? Descripcion) : IRequest;
