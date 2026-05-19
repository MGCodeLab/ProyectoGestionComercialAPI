using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.ParametroSistema.Actualizar;
using Application.Features.Catalogo.ParametroSistema.ActualizarEstado;
using Application.Features.Catalogo.ParametroSistema.Crear;
using Application.Features.Catalogo.ParametroSistema.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ParametrosSistemaController : ControllerBase
{
    private readonly IParametroSistemaService _service;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ParametrosSistemaController(IParametroSistemaService service, IMediator mediator, IMapper mapper)
    {
        _service = service;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(CancellationToken token)
    {
        var parametros = await _service.ObtenerTodos(token);
        var parametrosDto = _mapper.Map<List<ParametroSistemaDto>>(parametros);
        return this.OkResponse(parametrosDto, "Parámetros obtenidos exitosamente");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken token)
    {
        var parametro = await _service.ObtenerPorId(id, false, token);
        if (parametro == null)
            return this.NotFoundResponse("Parámetro no encontrado");
        var parametroDto = _mapper.Map<ParametroSistemaDto>(parametro);
        return this.OkResponse(parametroDto, "Parámetro obtenido exitosamente");
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearParametroSistemaDto dto, CancellationToken token)
    {
        var command = _mapper.Map<CrearParametroSistemaCommand>(dto);
        var id = await _mediator.Send(command, token);
        var parametro = await _service.ObtenerPorId(id, false, token);
        var parametroDto = _mapper.Map<ParametroSistemaDto>(parametro);
        return this.CreatedResponse(nameof(ObtenerPorId), new { id }, parametroDto, "Parámetro creado exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarParametroSistemaDto dto, CancellationToken token)
    {
        var command = new ActualizarParametroSistemaCommand(id, dto.Clave, dto.Valor, dto.TipoDato, dto.Descripcion);
        await _mediator.Send(command, token);
        var parametro = await _service.ObtenerPorId(id, false, token);
        var parametroDto = _mapper.Map<ParametroSistemaDto>(parametro);
        return this.OkResponse(parametroDto, "Parámetro actualizado exitosamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoParametroSistemaCommand(id, true);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Parámetro activado exitosamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoParametroSistemaCommand(id, false);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Parámetro inactivado exitosamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken token)
    {
        var parametro = await _service.ObtenerPorId(id, false, token);
        if (parametro == null)
            return this.NotFoundResponse("Parámetro no encontrado");
        var command = new EliminarParametroSistemaCommand(id);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Parámetro eliminado exitosamente");
    }
}
