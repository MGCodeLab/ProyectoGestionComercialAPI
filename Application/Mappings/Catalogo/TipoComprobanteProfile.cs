using Application.Dtos.Catalogo;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo
{
    public class TipoComprobanteProfile : Profile
    {
        public TipoComprobanteProfile()
        {
            CreateMap<TipoComprobante, TipoComprobanteDto>();
            CreateMap<CrearTipoComprobanteDto, TipoComprobante>();
            CreateMap<ActualizarTipoComprobanteDto, TipoComprobante>();
        }
    }
}
