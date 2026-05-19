using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos.Comercial;
using Application.Features.Comercial.Proveedor.Actualizar;
using Application.Features.Comercial.Proveedor.ActualizarEstado;
using Application.Features.Comercial.Proveedor.Crear;
using Application.Features.Comercial.Proveedor.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProveedoresController : ControllerBase
{
    private readonly IProveedorService _service;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProveedoresController(IProveedorService service, IMediator mediator, IMapper mapper)
    {
        _service = service;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(CancellationToken token)
    {
        var proveedores = await _service.ObtenerTodos(token);
        var proveedoresDto = _mapper.Map<List<ProveedorDto>>(proveedores);
        return this.OkResponse(proveedoresDto, "Proveedores obtenidos exitosamente");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken token)
    {
        var proveedor = await _service.ObtenerPorId(id, token);
        if (proveedor == null)
            return this.NotFoundResponse("Proveedor no encontrado");
        var proveedorDto = _mapper.Map<ProveedorDto>(proveedor);
        return this.OkResponse(proveedorDto, "Proveedor obtenido exitosamente");
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearProveedorDto dto, CancellationToken token)
    {
        var command = _mapper.Map<CrearProveedorCommand>(dto);
        var id = await _mediator.Send(command, token);
        var proveedor = await _service.ObtenerPorId(id, token);
        var proveedorDto = _mapper.Map<ProveedorDto>(proveedor);
        return this.CreatedResponse(nameof(ObtenerPorId), new { id }, proveedorDto, "Proveedor creado exitosamente");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarProveedorDto dto, CancellationToken token)
    {
        var command = new ActualizarProveedorCommand(id, dto.TipoDocumentoId, dto.NumeroDocumento, dto.RazonSocial,
            dto.NombreComercial, dto.PaisId, dto.Correo, dto.Telefono, dto.Direccion);
        await _mediator.Send(command, token);
        var proveedor = await _service.ObtenerPorId(id, token);
        var proveedorDto = _mapper.Map<ProveedorDto>(proveedor);
        return this.OkResponse(proveedorDto, "Proveedor actualizado exitosamente");
    }

    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoProveedorCommand(id, true);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Proveedor activado exitosamente");
    }

    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id, CancellationToken token)
    {
        var command = new ActualizarEstadoProveedorCommand(id, false);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Proveedor inactivado exitosamente");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken token)
    {
        var proveedor = await _service.ObtenerPorId(id, token);
        if (proveedor == null)
            return this.NotFoundResponse("Proveedor no encontrado");
        var command = new EliminarProveedorCommand(id);
        await _mediator.Send(command, token);
        return this.OkResponse<string?>(null, "Proveedor eliminado exitosamente");
    }
}
