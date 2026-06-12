using Application.Dtos.Catalogo;
using Application.Features.Catalogo.TipoImpuesto.Actualizar;
using Application.Features.Catalogo.TipoImpuesto.ActualizarEstado;
using Application.Features.Catalogo.TipoImpuesto.Crear;
using Application.Features.Catalogo.TipoImpuesto.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TiposImpuestoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITipoImpuestoService _service;
    private readonly IMapper _mapper;

    public TiposImpuestoController(IMediator mediator, ITipoImpuestoService service, IMapper mapper)
    {
        _mediator = mediator;
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var datos = await _service.ObtenerTodos(HttpContext.RequestAborted);
        var response = _mapper.Map<List<TipoImpuestoDto>>(datos);
        return this.OkResponse(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var dato = await _service.ObtenerPorId(id, isAsTracking: false, HttpContext.RequestAborted);
        if (dato == null)
            return this.NotFoundResponse("TipoImpuesto no encontrado");

        var response = _mapper.Map<TipoImpuestoDto>(dato);
        return this.OkResponse(response);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTipoImpuestoDto dto)
    {
        var command = _mapper.Map<CrearTipoImpuestoCommand>(dto);
        var id = await _mediator.Send(command);
        return this.CreatedResponse(nameof(Obtener), new { id }, new { id, nombre = dto.Nombre }, "TipoImpuesto creado exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarTipoImpuestoDto dto)
    {
        var command = _mapper.Map<ActualizarTipoImpuestoCommand>(dto);
        command = command with { Id = id };
        await _mediator.Send(command);
        return this.OkResponse(string.Empty, "TipoImpuesto actualizado correctamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id)
    {
        await _mediator.Send(new ActualizarEstadoTipoImpuestoCommand(true, id));
        return this.OkResponse(string.Empty, "TipoImpuesto activado correctamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id)
    {
        await _mediator.Send(new ActualizarEstadoTipoImpuestoCommand(false, id));
        return this.OkResponse(string.Empty, "TipoImpuesto inactivado correctamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _mediator.Send(new EliminarTipoImpuestoCommand(id));
        return this.OkResponse(string.Empty, "TipoImpuesto eliminado correctamente");
    }
}
