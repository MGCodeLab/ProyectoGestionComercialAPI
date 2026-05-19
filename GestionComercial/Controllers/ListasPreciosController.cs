using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos.Catalogo;
using Application.Features.Catalogo.ListaPrecio.Actualizar;
using Application.Features.Catalogo.ListaPrecio.ActualizarEstado;
using Application.Features.Catalogo.ListaPrecio.Crear;
using Application.Features.Catalogo.ListaPrecio.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ListasPreciosController : ControllerBase
{
    private readonly IListaPrecioService _service;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ListasPreciosController(IListaPrecioService service, IMediator mediator, IMapper mapper)
    {
        _service = service;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(CancellationToken token)
    {
        var listas = await _service.ObtenerTodos(token);
        var listasDto = _mapper.Map<List<ListaPrecioDto>>(listas);
        return this.OkResponse(listasDto, "Listas de precios obtenidas exitosamente");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken token)
    {
        var lista = await _service.ObtenerPorId(id, token);
        if (lista == null)
            return this.NotFoundResponse("Lista de precios no encontrada");
        var listaDto = _mapper.Map<ListaPrecioDto>(lista);
        return this.OkResponse(listaDto, "Lista de precios obtenida exitosamente");
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearListaPrecioDto dto, CancellationToken token)
    {
        var command = _mapper.Map<CrearListaPrecioCommand>(dto);
        var id = await _mediator.Send(command, token);
        var lista = await _service.ObtenerPorId(id, token);
        var listaDto = _mapper.Map<ListaPrecioDto>(lista);
        return this.CreatedResponse(nameof(ObtenerPorId), new { id }, listaDto, "Lista de precios creada exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarListaPrecioDto dto, CancellationToken token)
    {
        var command = new ActualizarListaPrecioCommand(id, dto.Nombre, dto.MonedaId, dto.Descripcion, dto.EsDefault);
        await _mediator.Send(command, token);
        var lista = await _service.ObtenerPorId(id, token);
        var listaDto = _mapper.Map<ListaPrecioDto>(lista);
        return this.OkResponse(listaDto, "Lista de precios actualizada exitosamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoListaPrecioCommand(id, true);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Lista de precios activada exitosamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoListaPrecioCommand(id, false);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Lista de precios inactivada exitosamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken token)
    {
        var lista = await _service.ObtenerPorId(id, token);
        if (lista == null)
            return this.NotFoundResponse("Lista de precios no encontrada");
        var command = new EliminarListaPrecioCommand(id);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Lista de precios eliminada exitosamente");
    }
}
