using Application.Dtos.Catalogo;
using Application.Features.Catalogo.SerieDocumento.Actualizar;
using Application.Features.Catalogo.SerieDocumento.ActualizarEstado;
using Application.Features.Catalogo.SerieDocumento.Crear;
using Application.Features.Catalogo.SerieDocumento.Eliminar;
using Application.Features.Catalogo.SerieDocumento.ObtenerProximoNumero;
using Application.Interfaces;
using API.GestionComercial.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SeriesDocumentoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISerieDocumentoService _service;
    private readonly IMapper _mapper;

    public SeriesDocumentoController(IMediator mediator, ISerieDocumentoService service, IMapper mapper)
    {
        _mediator = mediator;
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var datos = await _service.ObtenerTodos(HttpContext.RequestAborted);
        var response = _mapper.Map<List<SerieDocumentoDto>>(datos);
        return this.OkResponse(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var dato = await _service.ObtenerPorId(id, isAsTracking: false, HttpContext.RequestAborted);
        if (dato == null)
            return this.NotFoundResponse("SerieDocumento no encontrado");

        var response = _mapper.Map<SerieDocumentoDto>(dato);
        return this.OkResponse(response);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearSerieDocumentoDto dto)
    {
        var command = _mapper.Map<CrearSerieDocumentoCommand>(dto);
        var id = await _mediator.Send(command);
        return this.CreatedResponse(nameof(Obtener), new { id }, new { id, serie = dto.Serie }, "SerieDocumento creado exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarSerieDocumentoDto dto)
    {
        var command = _mapper.Map<ActualizarSerieDocumentoCommand>(dto);
        command = command with { Id = id };
        await _mediator.Send(command);
        return this.OkResponse(string.Empty, "SerieDocumento actualizado correctamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id)
    {
        await _mediator.Send(new ActualizarEstadoSerieDocumentoCommand(true, id));
        return this.OkResponse(string.Empty, "SerieDocumento activado correctamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id)
    {
        await _mediator.Send(new ActualizarEstadoSerieDocumentoCommand(false, id));
        return this.OkResponse(string.Empty, "SerieDocumento inactivado correctamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _mediator.Send(new EliminarSerieDocumentoCommand(id));
        return this.OkResponse(string.Empty, "SerieDocumento eliminado correctamente");
    }

    [HttpGet("{id}/next-numero")]
    public async Task<IActionResult> ObtenerProximoNumero(int id)
    {
        var numero = await _mediator.Send(new ObtenerProximoNumeroQuery { SerieDocumentoId = id });
        return this.OkResponse(new { numero });
    }
}
