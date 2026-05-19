using MediatR;

namespace Application.Features.Organizacion.Sucursal.Crear
{
    public record CrearSucursalCommand(
        string Nombre,
        string Codigo,
        int EmpresaId,
        int PaisId,
        string? Direccion,
        string? Telefono,
        bool EsPrincipal
    ) : IRequest<int>;
}
