using Application.Dtos.Catalogo;
using Application.Features.Catalogo.TipoDocumento.Actualizar;
using Application.Features.Catalogo.TipoDocumento.Crear;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo;

public class TipoDocumentoProfile : Profile
{
    public TipoDocumentoProfile()
    {
        CreateMap<CrearTipoDocumentoDto, CrearTipoDocumentoCommand>();
        CreateMap<CrearTipoDocumentoCommand, TipoDocumento>();
        CreateMap<ActualizarTipoDocumentoDto, ActualizarTipoDocumentoCommand>();
        CreateMap<ActualizarTipoDocumentoCommand, TipoDocumento>().ReverseMap();
        CreateMap<TipoDocumento, TipoDocumentoDto>();
    }
}
