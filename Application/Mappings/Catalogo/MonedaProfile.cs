using AutoMapper;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.Moneda.Actualizar;
using Application.Features.Catalogo.Moneda.Crear;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo;

public class MonedaProfile : Profile
{
    public MonedaProfile()
    {
        CreateMap<Moneda, MonedaDto>().ReverseMap();
        CreateMap<CrearMonedaDto, CrearMonedaCommand>();
        CreateMap<CrearMonedaCommand, Moneda>();
        CreateMap<ActualizarMonedaCommand, Moneda>().ReverseMap();
    }
}
