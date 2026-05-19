using MediatR;

namespace Application.Features.Catalogo.ParametroSistema.Crear;

public record CrearParametroSistemaCommand(string Clave, string Valor, string TipoDato, string? Descripcion) : IRequest<int>;
