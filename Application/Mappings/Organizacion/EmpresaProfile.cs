using AutoMapper;
using Domain.Organizacion;
using Application.Dtos.Organizacion;
using Application.Features.Organizacion.Empresa.Crear;
using Application.Features.Organizacion.Empresa.Actualizar;

namespace Application.Mappings.Organizacion
{
    public class EmpresaProfile : Profile
    {
        public EmpresaProfile()
        {
            CreateMap<CrearEmpresaCommand, Empresa>();
            CreateMap<CrearEmpresaDto, CrearEmpresaCommand>();

            CreateMap<ActualizarEmpresaCommand, Empresa>();
            CreateMap<ActualizarEmpresaDto, ActualizarEmpresaCommand>();

            CreateMap<Empresa, EmpresaDto>();
            CreateMap<EmpresaDto, Empresa>();
        }
    }
}
