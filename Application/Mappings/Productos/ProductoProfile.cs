using Application.Dtos.Producto;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Productos
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            CreateMap<Producto, ProductoDto>().ReverseMap();
            CreateMap<CrearProductoDto, Producto>();
            CreateMap<ActualizarProductoDto, Producto>();
            CreateMap<ProductoDetalleDto, Producto>();
        }
    }
}
