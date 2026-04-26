using Application.Dtos.Cliente;
using Application.Features.Clientes.Actualizar;
using Application.Features.Clientes.ActualizarEstado;
using Application.Features.Clientes.Crear;
using Application.Features.Clientes.Eliminar;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IClienteService _service;
        private readonly IMediator _mediator;

        public ClientesController(
            IMapper mapper,
            IClienteService clienteService,
            IMediator mediator)
        {
            _mapper = mapper;
            _service = clienteService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var clientes = await _service.ObtenerTodos(HttpContext.RequestAborted);
            var result = _mapper.Map<List<ClienteDto>>(clientes);
            return this.OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _service.ObtenerPorId(id, isAsTracking: false, HttpContext.RequestAborted);

            if (cliente == null)
                return this.NotFoundResponse("Cliente no encontrado");

            var result = _mapper.Map<ClienteDto>(cliente);
            return this.OkResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCliente(CrearClienteDto dto)
        {
            var command = _mapper.Map<CrearClienteCommand>(dto);

            var id = await _mediator.Send(command);

            var result = new { id, nombres = dto.Nombres };

            return this.CreatedResponse(
                nameof(GetById),
                new { id },
                result,
                "Cliente creado exitosamente");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarClienteDto dto)
        {
            var command = _mapper.Map<ActualizarClienteCommand>(dto);
            command = command with { Id = id };

            await _mediator.Send(command);

            return this.OkResponse(string.Empty, "Cliente actualizado correctamente");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new EliminarClienteCommand(id));
            return this.OkResponse(string.Empty, "Cliente eliminado correctamente");
        }

        [HttpPatch("{id}/inactivar")]
        public async Task<IActionResult> Inactivar(int id)
        {
            await _mediator.Send(new ActualizarEstadoClienteCommand(id, false));

            return this.OkResponse(string.Empty, "Cliente inactivado correctamente");
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            await _mediator.Send(new ActualizarEstadoClienteCommand(id, true));

            return this.OkResponse(string.Empty, "Cliente activado correctamente");
        }
    }
}
