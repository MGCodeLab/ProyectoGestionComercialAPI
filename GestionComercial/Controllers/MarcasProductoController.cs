using Application.Dtos.Catalogo;
using Application.Features.Catalogo.MarcaProducto.Actualizar;
using Application.Features.Catalogo.MarcaProducto.ActualizarEstado;
using Application.Features.Catalogo.MarcaProducto.Crear;
using Application.Features.Catalogo.MarcaProducto.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MarcasProductoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMarcaProductoService _service;
    private readonly IMapper _mapper;

    public MarcasProductoController(IMediator mediator, IMarcaProductoService service, IMapper mapper)
    {
        _mediator = mediator;
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var datos = await _service.ObtenerTodosAsync(cancellationToken);
        var response = _mapper.Map<List<MarcaProductoDto>>(datos);
        return this.OkResponse(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id, CancellationToken cancellationToken)
    {
        var dato = await _service.ObtenerPorIdAsync(id, tracking: false, cancellationToken);
        if (dato == null)
            return this.NotFoundResponse("MarcaProducto no encontrada");

        var response = _mapper.Map<MarcaProductoDto>(dato);
        return this.OkResponse(response);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearMarcaProductoDto dto, CancellationToken cancellationToken)
    {
        var command = new CrearMarcaProductoCommand(dto.Nombre, dto.Descripcion, dto.LogoUrl);
        var id = await _mediator.Send(command, cancellationToken);
        return this.CreatedResponse(nameof(Obtener), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarMarcaProductoDto dto, CancellationToken cancellationToken)
    {
        var command = new ActualizarMarcaProductoCommand(id, dto.Nombre, dto.Descripcion, dto.LogoUrl);
        await _mediator.Send(command, cancellationToken);
        return this.OkResponse<object>(null, "MarcaProducto actualizada correctamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken cancellationToken)
    {
        var command = new ActualizarEstadoMarcaProductoCommand(id, true);
        await _mediator.Send(command, cancellationToken);
        return this.OkResponse<object>(null, "MarcaProducto activada correctamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id, CancellationToken cancellationToken)
    {
        var command = new ActualizarEstadoMarcaProductoCommand(id, false);
        await _mediator.Send(command, cancellationToken);
        return this.OkResponse<object>(null, "MarcaProducto inactivada correctamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        var command = new EliminarMarcaProductoCommand(id);
        await _mediator.Send(command, cancellationToken);
        return this.OkResponse<object>(null, "MarcaProducto eliminada correctamente");
    }
}
