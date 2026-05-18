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
            var datos = await _service.ObtenerTodosAsync();
            var response = _mapper.Map<List<SerieDocumentoDto>>(datos);
            return this.OkResponse(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var dato = await _service.ObtenerPorIdAsync(id);
            if (dato == null)
                return this.NotFoundResponse("SerieDocumento no encontrado");

            var response = _mapper.Map<SerieDocumentoDto>(dato);
            return this.OkResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearSerieDocumentoDto dto)
        {
            var command = new CrearSerieDocumentoCommand(dto.TipoComprobanteId, dto.SucursalId, dto.Serie, dto.NumeroMaximo);
            var id = await _mediator.Send(command);
            return this.CreatedResponse(nameof(Obtener), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarSerieDocumentoDto dto)
        {
            var command = new ActualizarSerieDocumentoCommand(dto.TipoComprobanteId, dto.SucursalId, dto.Serie, dto.NumeroMaximo, id);
            await _mediator.Send(command);
            return this.OkResponse<object>(null, "SerieDocumento actualizado correctamente");
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            var command = new ActualizarEstadoSerieDocumentoCommand(true, id);
            await _mediator.Send(command);
            return this.OkResponse<object>(null, "SerieDocumento activado correctamente");
        }

        [HttpPatch("{id}/inactivar")]
        public async Task<IActionResult> Inactivar(int id)
        {
            var command = new ActualizarEstadoSerieDocumentoCommand(false, id);
            await _mediator.Send(command);
            return this.OkResponse<object>(null, "SerieDocumento inactivado correctamente");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var command = new EliminarSerieDocumentoCommand(id);
            await _mediator.Send(command);
            return this.OkResponse<object>(null, "SerieDocumento eliminado correctamente");
        }

        [HttpGet("{id}/next-numero")]
        public async Task<IActionResult> ObtenerProximoNumero(int id)
        {
            var numero = await _mediator.Send(new ObtenerProximoNumeroQuery { SerieDocumentoId = id });
            return this.OkResponse(new { numero });
        }
    }
