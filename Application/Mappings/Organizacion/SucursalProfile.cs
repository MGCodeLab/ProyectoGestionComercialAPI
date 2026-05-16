using AutoMapper;
using Domain.Organizacion;
using Application.Dtos.Organizacion;
using Application.Features.Organizacion.Sucursal.Crear;
using Application.Features.Organizacion.Sucursal.Actualizar;

namespace Application.Mappings.Organizacion
{
    public class SucursalProfile : Profile
    {
        public SucursalProfile()
        {
            CreateMap<CrearSucursalCommand, Sucursal>();
            CreateMap<CrearSucursalDto, CrearSucursalCommand>();

            CreateMap<ActualizarSucursalCommand, Sucursal>();
            CreateMap<ActualizarSucursalDto, ActualizarSucursalCommand>();

            CreateMap<Sucursal, SucursalDto>();
            CreateMap<SucursalDto, Sucursal>();
        }
    }
}
