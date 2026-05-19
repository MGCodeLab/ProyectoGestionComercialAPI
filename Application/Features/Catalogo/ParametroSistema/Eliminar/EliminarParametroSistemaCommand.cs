using MediatR;

namespace Application.Features.Catalogo.ParametroSistema.Eliminar;

public record EliminarParametroSistemaCommand(int Id) : IRequest;
