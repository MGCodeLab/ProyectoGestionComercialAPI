using Application.Dtos.Producto;
using Application.Features.Productos.Actualizar.Commands;
using Application.Features.Productos.Crear.Commands;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Productos
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            CreateMap<Producto, ProductoDto>().ReverseMap();

            CreateMap<CrearProductoDto, CrearProductoCommand>();
            CreateMap<CrearProductoDto, Producto>();
            CreateMap<CrearProductoCommand, Producto>()
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true));

            CreateMap<ActualizarProductoDto, Producto>();
            CreateMap<ActualizarProductoDto, ActualizarProductoCommand>();
            CreateMap<ActualizarProductoCommand, Producto>().ReverseMap();
        }
    }
}
