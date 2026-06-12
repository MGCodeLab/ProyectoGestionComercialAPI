using Application.Dtos.Catalogo;
using Application.Features.Catalogo.TipoComprobante.Actualizar;
using Application.Features.Catalogo.TipoComprobante.Crear;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo
{
    public class TipoComprobanteProfile : Profile
    {
        public TipoComprobanteProfile()
        {
            CreateMap<TipoComprobante, TipoComprobanteDto>().ReverseMap();

            CreateMap<CrearTipoComprobanteDto, CrearTipoComprobanteCommand>();
            CreateMap<CrearTipoComprobanteDto, TipoComprobante>();
            CreateMap<CrearTipoComprobanteCommand, TipoComprobante>();

            CreateMap<ActualizarTipoComprobanteDto, TipoComprobante>();
            CreateMap<ActualizarTipoComprobanteDto, ActualizarTipoComprobanteCommand>();
            CreateMap<ActualizarTipoComprobanteCommand, TipoComprobante>().ReverseMap();
        }
    }
}
