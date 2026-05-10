using AutoMapper;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.ModuloSistema.Crear;
using Domain.Configuracion;

namespace Application.Mappings.Catalogo;

public class ModuloSistemaProfile : Profile
{
    public ModuloSistemaProfile()
    {
        CreateMap<ModuloSistema, ModuloSistemaDto>().ReverseMap();
        CreateMap<CrearModuloSistemaDto, CrearModuloSistemaCommand>();
        CreateMap<CrearModuloSistemaCommand, ModuloSistema>();
    }
}
