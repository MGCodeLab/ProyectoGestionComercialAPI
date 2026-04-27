namespace Application.Dtos.Auth
{
    /// <summary>
    /// DTO con la respuesta de autenticación exitosa.
    /// Se retorna en POST /api/v1/auth/login después de validar credenciales.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>JWT token de autenticación Bearer.</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Segundos hasta que el token expira.</summary>
        public int ExpiresIn { get; set; }

        /// <summary>Perfil del usuario autenticado con roles y permisos.</summary>
        public UserProfileDto User { get; set; } = new();
    }
}
