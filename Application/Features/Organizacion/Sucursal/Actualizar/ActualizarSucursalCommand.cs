using MediatR;

namespace Application.Features.Organizacion.Sucursal.Actualizar
{
    public record ActualizarSucursalCommand(
        string Nombre,
        string Codigo,
        int EmpresaId,
        int PaisId,
        string? Direccion,
        string? Telefono,
        bool EsPrincipal,
        int Id = 0
    ) : IRequest<int>;
}
