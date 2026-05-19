using Application.Dtos.Catalogo;
using Application.Features.Catalogo.TipoComprobante.Actualizar;
using Application.Features.Catalogo.TipoComprobante.ActualizarEstado;
using Application.Features.Catalogo.TipoComprobante.Crear;
using Application.Features.Catalogo.TipoComprobante.Eliminar;
using Application.Interfaces;
using API.GestionComercial.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TiposComprobanteController : ControllerBase
{
        private readonly IMediator _mediator;
        private readonly ITipoComprobanteService _service;
        private readonly IMapper _mapper;

        public TiposComprobanteController(IMediator mediator, ITipoComprobanteService service, IMapper mapper)
        {
            _mediator = mediator;
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var datos = await _service.ObtenerTodosAsync();
            var response = _mapper.Map<List<TipoComprobanteDto>>(datos);
            return this.OkResponse(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var dato = await _service.ObtenerPorIdAsync(id);
            if (dato == null)
                return this.NotFoundResponse("TipoComprobante no encontrado");

            var response = _mapper.Map<TipoComprobanteDto>(dato);
            return this.OkResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearTipoComprobanteDto dto)
        {
            var command = new CrearTipoComprobanteCommand(dto.Nombre, dto.Codigo, dto.AfectaInventario, dto.AfectaContable);
            var id = await _mediator.Send(command);
            return this.CreatedResponse(nameof(Obtener), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarTipoComprobanteDto dto)
        {
            var command = new ActualizarTipoComprobanteCommand(dto.Nombre, dto.Codigo, dto.AfectaInventario, dto.AfectaContable, id);
            await _mediator.Send(command);
            return this.OkResponse<object>(null, "TipoComprobante actualizado correctamente");
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            var command = new ActualizarEstadoTipoComprobanteCommand(true, id);
            await _mediator.Send(command);
            return this.OkResponse<object>(null, "TipoComprobante activado correctamente");
        }

        [HttpPatch("{id}/inactivar")]
        public async Task<IActionResult> Inactivar(int id)
        {
            var command = new ActualizarEstadoTipoComprobanteCommand(false, id);
            await _mediator.Send(command);
            return this.OkResponse<object>(null, "TipoComprobante inactivado correctamente");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var command = new EliminarTipoComprobanteCommand(id);
            await _mediator.Send(command);
            return this.OkResponse<object>(null, "TipoComprobante eliminado correctamente");
        }
    }
