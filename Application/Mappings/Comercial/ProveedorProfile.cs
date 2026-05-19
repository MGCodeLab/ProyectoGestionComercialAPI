using AutoMapper;
using Application.Dtos.Comercial;
using Application.Features.Comercial.Proveedor.Actualizar;
using Application.Features.Comercial.Proveedor.Crear;
using Domain.Comercial;

namespace Application.Mappings.Comercial;

public class ProveedorProfile : Profile
{
    public ProveedorProfile()
    {
        CreateMap<Proveedor, ProveedorDto>()
            .ForMember(dest => dest.TipoDocumentoCodigo, opt => opt.MapFrom(src => src.TipoDocumento.Codigo))
            .ForMember(dest => dest.PaisNombre, opt => opt.MapFrom(src => src.Pais.Nombre));

        CreateMap<ProveedorDto, Proveedor>()
            .ForMember(dest => dest.TipoDocumento, opt => opt.Ignore())
            .ForMember(dest => dest.Pais, opt => opt.Ignore());

        CreateMap<CrearProveedorDto, CrearProveedorCommand>();
        CreateMap<CrearProveedorCommand, Proveedor>();
        CreateMap<ActualizarProveedorCommand, Proveedor>();
    }
}
