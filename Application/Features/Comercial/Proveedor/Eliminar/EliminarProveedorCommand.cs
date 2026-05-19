using MediatR;

namespace Application.Features.Comercial.Proveedor.Eliminar;

public record EliminarProveedorCommand(int Id) : IRequest<int>;
