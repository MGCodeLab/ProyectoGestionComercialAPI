using Application.Dtos.Catalogo;
using Application.Features.Catalogo.TipoImpuesto.Actualizar;
using Application.Features.Catalogo.TipoImpuesto.Crear;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo
{
    public class TipoImpuestoProfile : Profile
    {
        public TipoImpuestoProfile()
        {
            CreateMap<TipoImpuesto, TipoImpuestoDto>().ReverseMap();

            CreateMap<CrearTipoImpuestoDto, CrearTipoImpuestoCommand>();
            CreateMap<CrearTipoImpuestoDto, TipoImpuesto>();
            CreateMap<CrearTipoImpuestoCommand, TipoImpuesto>();

            CreateMap<ActualizarTipoImpuestoDto, TipoImpuesto>();
            CreateMap<ActualizarTipoImpuestoDto, ActualizarTipoImpuestoCommand>();
            CreateMap<ActualizarTipoImpuestoCommand, TipoImpuesto>().ReverseMap();
        }
    }
}
