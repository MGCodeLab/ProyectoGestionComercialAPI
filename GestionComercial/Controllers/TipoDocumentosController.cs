using Application.Dtos.Catalogo;
using Application.Features.Catalogo.TipoDocumento.Actualizar;
using Application.Features.Catalogo.TipoDocumento.ActualizarEstado;
using Application.Features.Catalogo.TipoDocumento.Crear;
using Application.Features.Catalogo.TipoDocumento.Eliminar;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TipoDocumentosController : ControllerBase
{
    private readonly ITipoDocumentoService _service;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public TipoDocumentosController(ITipoDocumentoService service, IMediator mediator, IMapper mapper)
    {
        _service = service;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(CancellationToken token)
    {
        var tiposDocumento = await _service.ObtenerTodos(token);
        var tiposDocumentoDto = _mapper.Map<List<TipoDocumentoDto>>(tiposDocumento);
        return this.OkResponse(tiposDocumentoDto, "Tipos de documento obtenidos exitosamente");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken token)
    {
        var tipoDocumento = await _service.ObtenerPorId(id, false, token);
        if (tipoDocumento == null)
            return this.NotFoundResponse("Tipo de documento no encontrado");
        var tipoDocumentoDto = _mapper.Map<TipoDocumentoDto>(tipoDocumento);
        return this.OkResponse(tipoDocumentoDto, "Tipo de documento obtenido exitosamente");
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTipoDocumentoDto dto, CancellationToken token)
    {
        var command = _mapper.Map<CrearTipoDocumentoCommand>(dto);
        var id = await _mediator.Send(command, token);
        var result = new TipoDocumentoDto
        {
            Id = id,
            Codigo = dto.Codigo,
            Descripcion = dto.Descripcion
        };
        return this.CreatedResponse(nameof(ObtenerPorId), new { id }, result, "Tipo de documento creado exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarTipoDocumentoDto dto, CancellationToken token)
    {
        var command = new ActualizarTipoDocumentoCommand(id, dto.Codigo, dto.Descripcion);
        await _mediator.Send(command, token);
        return this.OkResponse(string.Empty, "Tipo de documento actualizado exitosamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoTipoDocumentoCommand(id, true);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Tipo de documento activado exitosamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoTipoDocumentoCommand(id, false);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Tipo de documento inactivado exitosamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken token)
    {
        var command = new EliminarTipoDocumentoCommand(id);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Tipo de documento eliminado exitosamente");
    }
}
