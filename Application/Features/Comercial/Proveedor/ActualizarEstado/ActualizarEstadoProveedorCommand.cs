using MediatR;

namespace Application.Features.Comercial.Proveedor.ActualizarEstado;

public record ActualizarEstadoProveedorCommand(
    int Id,
    bool Activo
) : IRequest<int>;
