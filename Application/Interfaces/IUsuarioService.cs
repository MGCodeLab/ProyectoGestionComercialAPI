using Domain.Seguridad;

namespace Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> AutenticarUsuario(string email, string password, CancellationToken token);
        Task<Usuario?> ObtenerPorId(int id, CancellationToken token);
        Task ActualizarLastLogin(int usuarioId, CancellationToken token);
    }
}
