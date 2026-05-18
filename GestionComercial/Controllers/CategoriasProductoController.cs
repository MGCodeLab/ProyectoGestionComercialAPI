using Application.Dtos.Catalogo;
using Application.Features.Catalogo.CategoriaProducto.Actualizar;
using Application.Features.Catalogo.CategoriaProducto.ActualizarEstado;
using Application.Features.Catalogo.CategoriaProducto.Crear;
using Application.Features.Catalogo.CategoriaProducto.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriasProductoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICategoriaProductoService _service;
    private readonly IMapper _mapper;

    public CategoriasProductoController(IMediator mediator, ICategoriaProductoService service, IMapper mapper)
    {
        _mediator = mediator;
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var datos = await _service.ObtenerTodosAsync();
        var response = _mapper.Map<List<CategoriaProductoDto>>(datos);
        return this.OkResponse(response);
    }

    [HttpGet("raices")]
    public async Task<IActionResult> ObtenerRaices()
    {
        var datos = await _service.ObtenerRaicesAsync();
        var response = _mapper.Map<List<CategoriaProductoDto>>(datos);
        return this.OkResponse(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var dato = await _service.ObtenerPorIdAsync(id);
        if (dato == null)
            return this.NotFoundResponse("CategoriaProducto no encontrada");

        var response = _mapper.Map<CategoriaProductoDto>(dato);
        return this.OkResponse(response);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCategoriaProductoDto dto)
    {
        var command = new CrearCategoriaProductoCommand(dto.Nombre, dto.Descripcion, dto.CategoriaPadreId);
        var id = await _mediator.Send(command);
        return this.CreatedResponse(nameof(Obtener), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarCategoriaProductoDto dto)
    {
        var command = new ActualizarCategoriaProductoCommand(id, dto.Nombre, dto.Descripcion, dto.CategoriaPadreId);
        await _mediator.Send(command);
        return this.OkResponse<object>(null, "CategoriaProducto actualizada correctamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id)
    {
        var command = new ActualizarEstadoCategoriaProductoCommand(id, true);
        await _mediator.Send(command);
        return this.OkResponse<object>(null, "CategoriaProducto activada correctamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id)
    {
        var command = new ActualizarEstadoCategoriaProductoCommand(id, false);
        await _mediator.Send(command);
        return this.OkResponse<object>(null, "CategoriaProducto inactivada correctamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var command = new EliminarCategoriaProductoCommand(id);
        await _mediator.Send(command);
        return this.OkResponse<object>(null, "CategoriaProducto eliminada correctamente");
    }
}
