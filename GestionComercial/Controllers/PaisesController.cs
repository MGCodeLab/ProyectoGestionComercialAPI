using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.Pais.Actualizar;
using Application.Features.Catalogo.Pais.ActualizarEstado;
using Application.Features.Catalogo.Pais.Crear;
using Application.Features.Catalogo.Pais.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaisesController : ControllerBase
{
    private readonly IPaisService _service;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public PaisesController(IPaisService service, IMediator mediator, IMapper mapper)
    {
        _service = service;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(CancellationToken token)
    {
        var paises = await _service.ObtenerTodos(token);
        var paisesDto = _mapper.Map<List<PaisDto>>(paises);
        return this.OkResponse(paisesDto, "Países obtenidos exitosamente");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken token)
    {
        var pais = await _service.ObtenerPorId(id, true, token);
        if (pais == null)
            return this.NotFoundResponse("País no encontrado");
        var paisDto = _mapper.Map<PaisDto>(pais);
        return this.OkResponse(paisDto, "País obtenido exitosamente");
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearPaisDto dto, CancellationToken token)
    {
        var command = _mapper.Map<CrearPaisCommand>(dto);
        var id = await _mediator.Send(command, token);
        var pais = await _service.ObtenerPorId(id, false, token);
        var paisDto = _mapper.Map<PaisDto>(pais);
        return this.CreatedResponse(nameof(ObtenerPorId), new { id }, paisDto, "País creado exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarPaisDto dto, CancellationToken token)
    {
        var command = new ActualizarPaisCommand(id, dto.Nombre, dto.Codigo, dto.CodigoMoneda);
        await _mediator.Send(command, token);
        var pais = await _service.ObtenerPorId(id, false, token);
        var paisDto = _mapper.Map<PaisDto>(pais);
        return this.OkResponse(paisDto, "País actualizado exitosamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoPaisCommand(id, true);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "País activado exitosamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoPaisCommand(id, false);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "País inactivado exitosamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken token)
    {
        var pais = await _service.ObtenerPorId(id, false, token);
        if (pais == null)
            return this.NotFoundResponse("País no encontrado");
        var command = new EliminarPaisCommand(id);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "País eliminado exitosamente");
    }
}
