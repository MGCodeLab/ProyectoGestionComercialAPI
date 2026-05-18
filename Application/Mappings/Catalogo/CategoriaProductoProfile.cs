using Application.Dtos.Catalogo;
using Application.Features.Catalogo.CategoriaProducto.Actualizar;
using Application.Features.Catalogo.CategoriaProducto.Crear;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo
{
    public class CategoriaProductoProfile : Profile
    {
        public CategoriaProductoProfile()
        {
            CreateMap<CrearCategoriaProductoCommand, CategoriaProducto>();
            CreateMap<CrearCategoriaProductoDto, CrearCategoriaProductoCommand>();

            CreateMap<ActualizarCategoriaProductoCommand, CategoriaProducto>()
                .ForMember(d => d.Id, opt => opt.Ignore());
            CreateMap<ActualizarCategoriaProductoDto, ActualizarCategoriaProductoCommand>();

            CreateMap<CategoriaProducto, CategoriaProductoDto>()
                .ForMember(d => d.PublicId, opt => opt.MapFrom(s => s.PublicId.ToString()))
                .ForMember(d => d.Subcategorias, opt => opt.MapFrom(s => s.Subcategorias));
        }
    }
}
