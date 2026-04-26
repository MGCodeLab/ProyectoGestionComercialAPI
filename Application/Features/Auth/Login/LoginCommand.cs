using Application.Dtos.Auth;
using MediatR;

namespace Application.Features.Auth.Login
{
    public record LoginCommand(
        string Email,
        string Password
    ) : IRequest<LoginResponseDto>;
}
