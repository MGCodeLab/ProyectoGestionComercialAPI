using Application.Interfaces;
using Domain.Seguridad;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> AutenticarUsuario(string email, string password, CancellationToken token)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                        .ThenInclude(r => r!.RolPermisos)
                            .ThenInclude(rp => rp.Permiso)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.Activo, token);

            if (usuario is null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
                return null;

            return usuario;
        }

        public async Task<Usuario?> ObtenerPorId(int id, CancellationToken token)
            => await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                        .ThenInclude(r => r!.RolPermisos)
                            .ThenInclude(rp => rp.Permiso)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.Activo, token);

        public async Task ActualizarLastLogin(int usuarioId, CancellationToken token)
        {
            var usuario = await _context.Usuarios.FindAsync([usuarioId], token);
            if (usuario is not null)
            {
                usuario.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync(token);
            }
        }
    }
}
