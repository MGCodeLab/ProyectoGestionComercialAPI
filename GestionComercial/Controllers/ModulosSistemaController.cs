using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.ModuloSistema.Actualizar;
using Application.Features.Catalogo.ModuloSistema.ActualizarEstado;
using Application.Features.Catalogo.ModuloSistema.Crear;
using Application.Features.Catalogo.ModuloSistema.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ModulosSistemaController : ControllerBase
{
    private readonly IModuloSistemaService _service;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ModulosSistemaController(IModuloSistemaService service, IMediator mediator, IMapper mapper)
    {
        _service = service;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(CancellationToken token)
    {
        var modulos = await _service.ObtenerTodos(token);
        var modulosDto = _mapper.Map<List<ModuloSistemaDto>>(modulos);
        return this.OkResponse(modulosDto, "Módulos obtenidos exitosamente");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken token)
    {
        var modulo = await _service.ObtenerPorId(id, false, token);
        if (modulo == null)
            return this.NotFoundResponse("Módulo no encontrado");
        var moduloDto = _mapper.Map<ModuloSistemaDto>(modulo);
        return this.OkResponse(moduloDto, "Módulo obtenido exitosamente");
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearModuloSistemaDto dto, CancellationToken token)
    {
        var command = _mapper.Map<CrearModuloSistemaCommand>(dto);
        var id = await _mediator.Send(command, token);
        var modulo = await _service.ObtenerPorId(id, false, token);
        var moduloDto = _mapper.Map<ModuloSistemaDto>(modulo);
        return this.CreatedResponse(nameof(ObtenerPorId), new { id }, moduloDto, "Módulo creado exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarModuloSistemaDto dto, CancellationToken token)
    {
        var command = new ActualizarModuloSistemaCommand(id, dto.Nombre, dto.Codigo, dto.Descripcion);
        await _mediator.Send(command, token);
        var modulo = await _service.ObtenerPorId(id, false, token);
        var moduloDto = _mapper.Map<ModuloSistemaDto>(modulo);
        return this.OkResponse(moduloDto, "Módulo actualizado exitosamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoModuloSistemaCommand(id, true);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Módulo activado exitosamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoModuloSistemaCommand(id, false);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Módulo inactivado exitosamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken token)
    {
        var modulo = await _service.ObtenerPorId(id, false, token);
        if (modulo == null)
            return this.NotFoundResponse("Módulo no encontrado");
        var command = new EliminarModuloSistemaCommand(id);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Módulo eliminado exitosamente");
    }
}
