using MediatR;

namespace Application.Features.Catalogo.ParametroSistema.Actualizar;

public record ActualizarParametroSistemaCommand(int Id, string Clave, string Valor, string TipoDato, string? Descripcion) : IRequest;
