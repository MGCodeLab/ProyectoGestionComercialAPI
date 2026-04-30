using Application.Dtos.Cliente;
using Application.Features.Clientes.Actualizar;
using Application.Features.Clientes.Crear;
using AutoMapper;
using Domain.Comercial;

namespace Application.Mappings.Clientes
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<Cliente, ClienteDto>().ReverseMap();

            CreateMap<CrearClienteDto, CrearClienteCommand>();
            CreateMap<CrearClienteDto, Cliente>();
            CreateMap<CrearClienteCommand, Cliente>();

            CreateMap<ActualizarClienteDto, Cliente>();
            CreateMap<ActualizarClienteDto, ActualizarClienteCommand>();
            CreateMap<ActualizarClienteCommand, Cliente>().ReverseMap();
        }
    }
}
