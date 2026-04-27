namespace Application.Dtos.Auth
{
    /// <summary>
    /// DTO con el perfil del usuario autenticado.
    /// Incluye información básica, roles y permisos asignados.
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>Identificador único interno del usuario.</summary>
        public int Id { get; set; }

        /// <summary>Nombre completo del usuario.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Correo electrónico del usuario.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Lista de nombres de roles asignados al usuario (ej: ADMIN, VENDOR, READ_ONLY).</summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>Lista de permisos efectivos del usuario (resultado de todos sus roles).</summary>
        public List<PermissionDto> Permisos { get; set; } = new();
    }
}
