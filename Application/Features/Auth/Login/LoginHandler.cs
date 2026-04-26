using Application.Dtos.Auth;
using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(
            IUsuarioService usuarioService,
            IJwtService jwtService,
            ILogger<LoginHandler> logger)
        {
            _usuarioService = usuarioService;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login intento para: {Email}", request.Email);

            var emailNormalizado = request.Email.Trim().ToLower();

            var usuario = await _usuarioService.AutenticarUsuario(emailNormalizado, request.Password, cancellationToken);

            if (usuario is null)
                throw new UnauthorizedException("Email o contraseña incorrectos");

            var roles = usuario.UsuarioRoles
                .Select(ur => ur.Rol!.Nombre)
                .ToList();

            var permisos = usuario.UsuarioRoles
                .SelectMany(ur => ur.Rol!.RolPermisos)
                .Select(rp => (rp.Permiso!.Recurso, rp.Permiso.Accion))
                .Distinct()
                .ToList();

            var token = _jwtService.GenerarToken(usuario, roles, permisos);

            await _usuarioService.ActualizarLastLogin(usuario.Id, cancellationToken);

            _logger.LogInformation("Login exitoso para: {Email}", emailNormalizado);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresIn = _jwtService.ObtenerExpiracionEnSegundos(),
                User = new UserProfileDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Roles = roles,
                    Permisos = permisos
                        .Select(p => new PermissionDto { Recurso = p.Recurso, Accion = p.Accion })
                        .ToList()
                }
            };
        }
    }
}
