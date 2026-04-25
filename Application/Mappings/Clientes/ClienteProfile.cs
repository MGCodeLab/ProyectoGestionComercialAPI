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
            CreateMap<CrearClienteDto, CrearClienteCommand>();
            CreateMap<ActualizarClienteDto, ActualizarClienteCommand>();

            CreateMap<CrearClienteCommand, Cliente>();

            CreateMap<Cliente, ClienteDto>();
        }
    }
}
