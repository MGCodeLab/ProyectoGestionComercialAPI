using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.CondicionPago.Actualizar;
using Application.Features.Catalogo.CondicionPago.ActualizarEstado;
using Application.Features.Catalogo.CondicionPago.Crear;
using Application.Features.Catalogo.CondicionPago.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CondicionesPagoController : ControllerBase
{
    private readonly ICondicionPagoService _service;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CondicionesPagoController(ICondicionPagoService service, IMediator mediator, IMapper mapper)
    {
        _service = service;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(CancellationToken token)
    {
        var condiciones = await _service.ObtenerTodos(token);
        var condicionesDto = _mapper.Map<List<CondicionPagoDto>>(condiciones);
        return this.OkResponse(condicionesDto, "Condiciones de pago obtenidas exitosamente");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken token)
    {
        var condicion = await _service.ObtenerPorId(id, token);
        if (condicion == null)
            return this.NotFoundResponse("Condición de pago no encontrada");
        var condicionDto = _mapper.Map<CondicionPagoDto>(condicion);
        return this.OkResponse(condicionDto, "Condición de pago obtenida exitosamente");
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCondicionPagoDto dto, CancellationToken token)
    {
        var command = _mapper.Map<CrearCondicionPagoCommand>(dto);
        var id = await _mediator.Send(command, token);
        var condicion = await _service.ObtenerPorId(id, token);
        var condicionDto = _mapper.Map<CondicionPagoDto>(condicion);
        return this.CreatedResponse(nameof(ObtenerPorId), new { id }, condicionDto, "Condición de pago creada exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarCondicionPagoDto dto, CancellationToken token)
    {
        var command = new ActualizarCondicionPagoCommand(id, dto.Nombre, dto.DiasCredito, dto.Descripcion);
        await _mediator.Send(command, token);
        var condicion = await _service.ObtenerPorId(id, token);
        var condicionDto = _mapper.Map<CondicionPagoDto>(condicion);
        return this.OkResponse(condicionDto, "Condición de pago actualizada exitosamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoCondicionPagoCommand(id, true);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Condición de pago activada exitosamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoCondicionPagoCommand(id, false);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Condición de pago inactivada exitosamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken token)
    {
        var condicion = await _service.ObtenerPorId(id, token);
        if (condicion == null)
            return this.NotFoundResponse("Condición de pago no encontrada");
        var command = new EliminarCondicionPagoCommand(id);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Condición de pago eliminada exitosamente");
    }
}
