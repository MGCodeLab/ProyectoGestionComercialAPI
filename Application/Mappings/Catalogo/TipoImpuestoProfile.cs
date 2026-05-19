using Application.Dtos.Catalogo;
using AutoMapper;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo
{
    public class TipoImpuestoProfile : Profile
    {
        public TipoImpuestoProfile()
        {
            CreateMap<TipoImpuesto, TipoImpuestoDto>();
            CreateMap<CrearTipoImpuestoDto, TipoImpuesto>();
            CreateMap<ActualizarTipoImpuestoDto, TipoImpuesto>();
        }
    }
}
