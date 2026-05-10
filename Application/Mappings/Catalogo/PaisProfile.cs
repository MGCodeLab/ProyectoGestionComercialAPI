using AutoMapper;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.Pais.Actualizar;
using Application.Features.Catalogo.Pais.Crear;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo;

public class PaisProfile : Profile
{
    public PaisProfile()
    {
        CreateMap<Pais, PaisDto>().ReverseMap();
        CreateMap<CrearPaisDto, CrearPaisCommand>();
        CreateMap<CrearPaisDto, Pais>();
        CreateMap<CrearPaisCommand, Pais>();
        CreateMap<ActualizarPaisDto, Pais>();
        CreateMap<ActualizarPaisDto, ActualizarPaisCommand>();
        CreateMap<ActualizarPaisCommand, Pais>().ReverseMap();
    }
}
