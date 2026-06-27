using AutoMapper;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.ParametroSistema.Crear;
using Application.Features.Catalogo.ParametroSistema.Actualizar;
using Domain.Configuracion;

namespace Application.Mappings.Catalogo;

public class ParametroSistemaProfile : Profile
{
    public ParametroSistemaProfile()
    {
        CreateMap<ParametroSistema, ParametroSistemaDto>().ReverseMap();
        CreateMap<CrearParametroSistemaDto, CrearParametroSistemaCommand>();
        CreateMap<CrearParametroSistemaCommand, ParametroSistema>();
        CreateMap<ActualizarParametroSistemaDto, ActualizarParametroSistemaCommand>();
        CreateMap<ActualizarParametroSistemaCommand, ParametroSistema>().ReverseMap();
    }
}
