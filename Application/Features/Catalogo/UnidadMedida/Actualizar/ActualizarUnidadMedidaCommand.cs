using MediatR;

namespace Application.Features.Catalogo.UnidadMedida.Actualizar;

public record ActualizarUnidadMedidaCommand(int Id, string Nombre, string Simbolo, string Codigo) : IRequest;
