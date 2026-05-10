using AutoMapper;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.UnidadMedida.Crear;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo;

public class UnidadMedidaProfile : Profile
{
    public UnidadMedidaProfile()
    {
        CreateMap<UnidadMedida, UnidadMedidaDto>().ReverseMap();
        CreateMap<CrearUnidadMedidaDto, CrearUnidadMedidaCommand>();
        CreateMap<CrearUnidadMedidaCommand, UnidadMedida>();
    }
}
