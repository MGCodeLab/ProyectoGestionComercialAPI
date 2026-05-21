using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.UnidadMedida.Actualizar;
using Application.Features.Catalogo.UnidadMedida.ActualizarEstado;
using Application.Features.Catalogo.UnidadMedida.Crear;
using Application.Features.Catalogo.UnidadMedida.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UnidadesMedidaController : ControllerBase
{
    private readonly IUnidadMedidaService _service;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UnidadesMedidaController(IUnidadMedidaService service, IMediator mediator, IMapper mapper)
    {
        _service = service;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(CancellationToken token)
    {
        var unidades = await _service.ObtenerTodos(token);
        var unidadesDto = _mapper.Map<List<UnidadMedidaDto>>(unidades);
        return this.OkResponse(unidadesDto, "Unidades de medida obtenidas exitosamente");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken token)
    {
        var unidad = await _service.ObtenerPorId(id, false, token);
        if (unidad == null)
            return this.NotFoundResponse("Unidad de medida no encontrada");
        var unidadDto = _mapper.Map<UnidadMedidaDto>(unidad);
        return this.OkResponse(unidadDto, "Unidad de medida obtenida exitosamente");
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUnidadMedidaDto dto, CancellationToken token)
    {
        var command = _mapper.Map<CrearUnidadMedidaCommand>(dto);
        var id = await _mediator.Send(command, token);
        var unidad = await _service.ObtenerPorId(id, false, token);
        var unidadDto = _mapper.Map<UnidadMedidaDto>(unidad);
        return this.CreatedResponse(nameof(ObtenerPorId), new { id }, unidadDto, "Unidad de medida creada exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUnidadMedidaDto dto, CancellationToken token)
    {
        var command = new ActualizarUnidadMedidaCommand(id, dto.Nombre, dto.Simbolo, dto.Codigo);
        await _mediator.Send(command, token);
        var unidad = await _service.ObtenerPorId(id, false, token);
        var unidadDto = _mapper.Map<UnidadMedidaDto>(unidad);
        return this.OkResponse(unidadDto, "Unidad de medida actualizada exitosamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoUnidadMedidaCommand(id, true);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Unidad de medida activada exitosamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoUnidadMedidaCommand(id, false);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Unidad de medida inactivada exitosamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken token)
    {
        var command = new EliminarUnidadMedidaCommand(id);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Unidad de medida eliminada exitosamente");
    }
}
