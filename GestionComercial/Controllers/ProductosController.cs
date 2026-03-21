using Application.Dtos.Producto;
using Application.Interfaces;
using AutoMapper;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.GestionComercial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductoService _service;

        public ProductosController(
            IMapper mapper,
            IProductoService productoService
            )
        {
            _mapper = mapper;
            _service = productoService;
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
            var producto = _mapper.Map<Producto>(dto);
            await _service.Crear(producto, HttpContext.RequestAborted);
            //return Ok(producto);
            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarProductoDto dto)
        {
            var producto = await _service.ObtenerPorId(id, isAsTracking: true, HttpContext.RequestAborted);

            if (producto == null)
                return NotFound();

            _mapper.Map(dto, producto);

            await _service.Actualizar(HttpContext.RequestAborted);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _service.ObtenerPorId(id, isAsTracking: false, HttpContext.RequestAborted);

            if (producto == null)
                return NotFound();

            await _service.Eliminar(producto, HttpContext.RequestAborted);

            return NoContent();
        }

        [HttpPatch("{id}/inactivar")]
        public async Task<IActionResult> Inactivar(int id)
        {
            var producto = await _service.ObtenerPorId(id, isAsTracking: true, HttpContext.RequestAborted);

            if (producto == null)
                return NotFound();

            producto.Activo = false;

            await _service.Actualizar(HttpContext.RequestAborted);

            return NoContent();
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            var producto = await _service.ObtenerPorId(id, isAsTracking: true, HttpContext.RequestAborted);

            if (producto == null)
                return NotFound();

            producto.Activo = true;

            await _service.Actualizar(HttpContext.RequestAborted);

            return NoContent();
        }


    }
}
