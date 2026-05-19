using MediatR;

namespace Application.Features.Catalogo.UnidadMedida.Crear;

public record CrearUnidadMedidaCommand(string Nombre, string Simbolo, string Codigo) : IRequest<int>;
