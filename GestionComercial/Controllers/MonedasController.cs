using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.Moneda.Actualizar;
using Application.Features.Catalogo.Moneda.ActualizarEstado;
using Application.Features.Catalogo.Moneda.Crear;
using Application.Features.Catalogo.Moneda.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MonedasController : ControllerBase
{
    private readonly IMonedaService _service;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public MonedasController(IMonedaService service, IMediator mediator, IMapper mapper)
    {
        _service = service;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(CancellationToken token)
    {
        var monedas = await _service.ObtenerTodos(token);
        var monedasDto = _mapper.Map<List<MonedaDto>>(monedas);
        return this.OkResponse(monedasDto, "Monedas obtenidas exitosamente");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken token)
    {
        var moneda = await _service.ObtenerPorId(id, false, token);
        if (moneda == null)
            return this.NotFoundResponse("Moneda no encontrada");
        var monedaDto = _mapper.Map<MonedaDto>(moneda);
        return this.OkResponse(monedaDto, "Moneda obtenida exitosamente");
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearMonedaDto dto, CancellationToken token)
    {
        var command = _mapper.Map<CrearMonedaCommand>(dto);
        var id = await _mediator.Send(command, token);
        var moneda = await _service.ObtenerPorId(id, false, token);
        var monedaDto = _mapper.Map<MonedaDto>(moneda);
        return this.CreatedResponse(nameof(ObtenerPorId), new { id }, monedaDto, "Moneda creada exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarMonedaDto dto, CancellationToken token)
    {
        var command = new ActualizarMonedaCommand(id, dto.Nombre, dto.Simbolo, dto.CodigoISO, dto.EsMonedaBase);
        await _mediator.Send(command, token);
        var moneda = await _service.ObtenerPorId(id, false, token);
        var monedaDto = _mapper.Map<MonedaDto>(moneda);
        return this.OkResponse(monedaDto, "Moneda actualizada exitosamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoMonedaCommand(id, true);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Moneda activada exitosamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoMonedaCommand(id, false);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Moneda inactivada exitosamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken token)
    {
        var command = new EliminarMonedaCommand(id);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Moneda eliminada exitosamente");
    }
}
