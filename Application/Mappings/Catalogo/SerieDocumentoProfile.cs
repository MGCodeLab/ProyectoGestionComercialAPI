using Application.Dtos.Catalogo;
using Application.Features.Catalogo.SerieDocumento.Actualizar;
using Application.Features.Catalogo.SerieDocumento.Crear;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo
{
    public class SerieDocumentoProfile : Profile
    {
        public SerieDocumentoProfile()
        {
            CreateMap<SerieDocumento, SerieDocumentoDto>().ReverseMap();

            CreateMap<CrearSerieDocumentoDto, CrearSerieDocumentoCommand>();
            CreateMap<CrearSerieDocumentoDto, SerieDocumento>();
            CreateMap<CrearSerieDocumentoCommand, SerieDocumento>();

            CreateMap<ActualizarSerieDocumentoDto, SerieDocumento>();
            CreateMap<ActualizarSerieDocumentoDto, ActualizarSerieDocumentoCommand>();
            CreateMap<ActualizarSerieDocumentoCommand, SerieDocumento>().ReverseMap();
        }
    }
}
