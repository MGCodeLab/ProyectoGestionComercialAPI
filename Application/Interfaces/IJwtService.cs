using Domain.Seguridad;

namespace Application.Interfaces
{
    public interface IJwtService
    {
        string GenerarToken(Usuario usuario, IList<string> roles, IList<(string Recurso, string Accion)> permisos);
        int ObtenerExpiracionEnSegundos();
    }
}
