using AutoMapper;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.ParametroSistema.Crear;
using Domain.Configuracion;

namespace Application.Mappings.Catalogo;

public class ParametroSistemaProfile : Profile
{
    public ParametroSistemaProfile()
    {
        CreateMap<ParametroSistema, ParametroSistemaDto>().ReverseMap();
        CreateMap<CrearParametroSistemaDto, CrearParametroSistemaCommand>();
        CreateMap<CrearParametroSistemaCommand, ParametroSistema>();
    }
}
