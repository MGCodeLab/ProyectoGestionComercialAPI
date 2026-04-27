using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Auth
{
    /// <summary>
    /// DTO para solicitar autenticación de usuario.
    /// Se envía en POST /api/v1/auth/login con credenciales del usuario.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>Correo electrónico del usuario a autenticar.</summary>
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo debe ser válido")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Contraseña del usuario en texto plano.</summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Password { get; set; } = string.Empty;
    }
}
