using Application.Dtos.Producto;
using Application.Features.Productos.Actualizar;
using Application.Features.Productos.ActualizarEstado;
using Application.Features.Productos.Crear;
using Application.Features.Productos.Eliminar.Commands;
using Application.Interfaces;
using AutoMapper;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Infrastructure.Repository;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.GestionComercial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductoService _service;
        private readonly IMediator _mediator;

        public ProductosController(
            IMapper mapper,
            IProductoService productoService
,
            IMediator mediator)
        {
            _mapper = mapper;
            _service = productoService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var productos = await _service.ObtenerTodos(HttpContext.RequestAborted);
            var result = _mapper.Map<List<ProductoDto>>(productos);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var producto = await _service.ObtenerPorId(id, isAsTracking: false, HttpContext.RequestAborted);

            if (producto == null)
                return NotFound();

            var result = _mapper.Map<ProductoDto>(producto);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CrearProductoDto dto)
        {
            var command = _mapper.Map<CrearProductoCommand>(dto);

            var id = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarProductoDto dto)
        {
            var command = _mapper.Map<ActualizarProductoCommand>(dto);
            command = command with { Id = id };

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new EliminarProductoCommand(id));
            return NoContent();
        }

        [HttpPatch("{id}/inactivar")]
        public async Task<IActionResult> Inactivar(int id)
        {
            await _mediator.Send(new ActualizarEstadoProductoCommand(id, false));

            return NoContent();
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            await _mediator.Send(new ActualizarEstadoProductoCommand(id, true));

            return NoContent();
        }
    }
}
