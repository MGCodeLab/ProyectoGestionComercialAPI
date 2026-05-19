using AutoMapper;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.ListaPrecio.Actualizar;
using Application.Features.Catalogo.ListaPrecio.Crear;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo;

public class ListaPrecioProfile : Profile
{
    public ListaPrecioProfile()
    {
        CreateMap<ListaPrecio, ListaPrecioDto>()
            .ForMember(dest => dest.MonedaNombre, opt => opt.MapFrom(src => src.Moneda.Nombre));

        CreateMap<ListaPrecioDto, ListaPrecio>()
            .ForMember(dest => dest.Moneda, opt => opt.Ignore());

        CreateMap<CrearListaPrecioDto, CrearListaPrecioCommand>();
        CreateMap<CrearListaPrecioCommand, ListaPrecio>();
        CreateMap<ActualizarListaPrecioCommand, ListaPrecio>();
    }
}
