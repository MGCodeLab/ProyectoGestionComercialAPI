using Application.Dtos.Producto;
using Application.Features.Productos.Actualizar;
using Application.Features.Productos.Crear;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Productos
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            // Sprint 4: Include new FK and navigation properties
            CreateMap<Producto, ProductoDto>()
                .ForMember(d => d.UnidadMedidaId, opt => opt.MapFrom(s => s.UnidadMedidaId))
                .ForMember(d => d.CategoriaProductoId, opt => opt.MapFrom(s => s.CategoriaProductoId))
                .ForMember(d => d.MarcaProductoId, opt => opt.MapFrom(s => s.MarcaProductoId))
                .ReverseMap();

            CreateMap<CrearProductoDto, CrearProductoCommand>();
            CreateMap<CrearProductoDto, Producto>();
            CreateMap<CrearProductoCommand, Producto>()
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true));

            CreateMap<ActualizarProductoDto, Producto>();
            CreateMap<ActualizarProductoDto, ActualizarProductoCommand>()
                .ForMember(d => d.UnidadMedidaId, opt => opt.MapFrom(s => s.UnidadMedidaId))
                .ForMember(d => d.CategoriaProductoId, opt => opt.MapFrom(s => s.CategoriaProductoId))
                .ForMember(d => d.MarcaProductoId, opt => opt.MapFrom(s => s.MarcaProductoId));
            CreateMap<ActualizarProductoCommand, Producto>().ReverseMap();
        }
    }
}
