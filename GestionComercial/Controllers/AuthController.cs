using Application.Dtos.Auth;
using Application.Features.Auth.Login;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IUsuarioService _usuarioService;

        public AuthController(IMapper mapper, IMediator mediator, IUsuarioService usuarioService)
        {
            _mapper = mapper;
            _mediator = mediator;
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var command = _mapper.Map<LoginCommand>(dto);
            var result = await _mediator.Send(command);
            return this.OkResponse(result, "Login exitoso");
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public IActionResult Logout()
            => this.OkResponse<object?>(null, "Sesión cerrada");

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
                return this.UnauthorizedResponse("Token inválido");

            var usuario = await _usuarioService.ObtenerPorId(userId, HttpContext.RequestAborted);

            if (usuario is null)
                return this.UnauthorizedResponse("Usuario no encontrado");

            var roles = usuario.UsuarioRoles.Select(ur => ur.Rol!.Nombre).ToList();
            var permisos = usuario.UsuarioRoles
                .SelectMany(ur => ur.Rol!.RolPermisos)
                .Select(rp => new PermissionDto { Recurso = rp.Permiso!.Recurso, Accion = rp.Permiso.Accion })
                .Distinct()
                .ToList();

            var profile = new UserProfileDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Roles = roles,
                Permisos = permisos
            };

            return this.OkResponse(profile);
        }
    }
}
