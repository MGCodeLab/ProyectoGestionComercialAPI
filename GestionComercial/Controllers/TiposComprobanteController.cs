using Application.Dtos.Catalogo;
using Application.Features.Catalogo.TipoComprobante.Actualizar;
using Application.Features.Catalogo.TipoComprobante.ActualizarEstado;
using Application.Features.Catalogo.TipoComprobante.Crear;
using Application.Features.Catalogo.TipoComprobante.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TiposComprobanteController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITipoComprobanteService _service;
    private readonly IMapper _mapper;

    public TiposComprobanteController(IMediator mediator, ITipoComprobanteService service, IMapper mapper)
    {
        _mediator = mediator;
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("combo/list")]
    public async Task<IActionResult> GetCombo()
    {
        var result = await _service.ObtenerCombo(HttpContext.RequestAborted);
        return this.OkResponse(result, "TiposComprobante para combo obtenidos exitosamente");
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var datos = await _service.ObtenerTodos(HttpContext.RequestAborted);
        var response = _mapper.Map<List<TipoComprobanteDto>>(datos);
        return this.OkResponse(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var dato = await _service.ObtenerPorId(id, isAsTracking: false, HttpContext.RequestAborted);
        if (dato == null)
            return this.NotFoundResponse("TipoComprobante no encontrado");

        var response = _mapper.Map<TipoComprobanteDto>(dato);
        return this.OkResponse(response);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTipoComprobanteDto dto)
    {
        var command = _mapper.Map<CrearTipoComprobanteCommand>(dto);
        var id = await _mediator.Send(command);
        return this.CreatedResponse(nameof(Obtener), new { id }, new { id, nombre = dto.Nombre }, "TipoComprobante creado exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarTipoComprobanteDto dto)
    {
        var command = _mapper.Map<ActualizarTipoComprobanteCommand>(dto);
        command = command with { Id = id };
        await _mediator.Send(command);
        return this.OkResponse(string.Empty, "TipoComprobante actualizado correctamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id)
    {
        await _mediator.Send(new ActualizarEstadoTipoComprobanteCommand(true, id));
        return this.OkResponse(string.Empty, "TipoComprobante activado correctamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id)
    {
        await _mediator.Send(new ActualizarEstadoTipoComprobanteCommand(false, id));
        return this.OkResponse(string.Empty, "TipoComprobante inactivado correctamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _mediator.Send(new EliminarTipoComprobanteCommand(id));
        return this.OkResponse(string.Empty, "TipoComprobante eliminado correctamente");
    }
}
