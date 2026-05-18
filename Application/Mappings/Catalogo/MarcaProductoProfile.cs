using Application.Dtos.Catalogo;
using Application.Features.Catalogo.MarcaProducto.Actualizar;
using Application.Features.Catalogo.MarcaProducto.Crear;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo
{
    public class MarcaProductoProfile : Profile
    {
        public MarcaProductoProfile()
        {
            CreateMap<CrearMarcaProductoCommand, MarcaProducto>();
            CreateMap<CrearMarcaProductoDto, CrearMarcaProductoCommand>();

            CreateMap<ActualizarMarcaProductoCommand, MarcaProducto>()
                .ForMember(d => d.Id, opt => opt.Ignore());
            CreateMap<ActualizarMarcaProductoDto, ActualizarMarcaProductoCommand>();

            CreateMap<MarcaProducto, MarcaProductoDto>()
                .ForMember(d => d.PublicId, opt => opt.MapFrom(s => s.PublicId.ToString()));
        }
    }
}
