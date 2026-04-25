using Application.Dtos.Auth;
using Application.Features.Auth.Login;
using AutoMapper;

namespace Application.Mappings.Auth
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<LoginRequestDto, LoginCommand>();
        }
    }
}
