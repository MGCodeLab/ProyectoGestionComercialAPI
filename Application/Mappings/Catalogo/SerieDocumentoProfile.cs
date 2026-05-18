using Application.Dtos.Catalogo;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo
{
    public class SerieDocumentoProfile : Profile
    {
        public SerieDocumentoProfile()
        {
            CreateMap<SerieDocumento, SerieDocumentoDto>();
            CreateMap<CrearSerieDocumentoDto, SerieDocumento>();
            CreateMap<ActualizarSerieDocumentoDto, SerieDocumento>();
        }
    }
}
