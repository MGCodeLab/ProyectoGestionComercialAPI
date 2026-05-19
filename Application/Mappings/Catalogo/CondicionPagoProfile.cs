using AutoMapper;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.CondicionPago.Actualizar;
using Application.Features.Catalogo.CondicionPago.Crear;
using Domain.Catalogo;

namespace Application.Mappings.Catalogo;

public class CondicionPagoProfile : Profile
{
    public CondicionPagoProfile()
    {
        CreateMap<CondicionPago, CondicionPagoDto>().ReverseMap();
        CreateMap<CrearCondicionPagoDto, CrearCondicionPagoCommand>();
        CreateMap<CrearCondicionPagoCommand, CondicionPago>();
        CreateMap<ActualizarCondicionPagoCommand, CondicionPago>();
    }
}
