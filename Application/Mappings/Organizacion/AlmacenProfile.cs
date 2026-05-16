using AutoMapper;
using Domain.Organizacion;
using Application.Dtos.Organizacion;
using Application.Features.Organizacion.Almacen.Crear;
using Application.Features.Organizacion.Almacen.Actualizar;

namespace Application.Mappings.Organizacion
{
    public class AlmacenProfile : Profile
    {
        public AlmacenProfile()
        {
            CreateMap<CrearAlmacenCommand, Almacen>();
            CreateMap<CrearAlmacenDto, CrearAlmacenCommand>();

            CreateMap<ActualizarAlmacenCommand, Almacen>();
            CreateMap<ActualizarAlmacenDto, ActualizarAlmacenCommand>();

            CreateMap<Almacen, AlmacenDto>();
            CreateMap<AlmacenDto, Almacen>();
        }
    }
}
