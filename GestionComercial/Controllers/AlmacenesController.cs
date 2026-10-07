using Application.Dtos.Organizacion;
using Application.Features.Organizacion.Almacen.Crear;
using Application.Features.Organizacion.Almacen.Actualizar;
using Application.Features.Organizacion.Almacen.ActualizarEstado;
using Application.Features.Organizacion.Almacen.Eliminar;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AlmacenesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAlmacenService _service;
        private readonly IMediator _mediator;

        public AlmacenesController(
            IMapper mapper,
            IAlmacenService service,
            IMediator mediator)
        {
            _mapper = mapper;
            _service = service;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(CancellationToken cancellationToken)
        {
            var result = await _service.ObtenerTodosOptimizado(cancellationToken);
            return this.OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.ObtenerPorIdOptimizado(id, cancellationToken);

            if (result == null)
                return this.NotFoundResponse("Almacén no encontrado");

            return this.OkResponse(result);
        }

        [HttpGet("combo/list")]
        public async Task<IActionResult> GetCombo(CancellationToken cancellationToken)
        {
            var result = await _service.ObtenerCombo(cancellationToken);
            return this.OkResponse(result, "Almacenes para combo obtenidos exitosamente");
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearAlmacenDto dto, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CrearAlmacenCommand>(dto);
            var id = await _mediator.Send(command, cancellationToken);
            var result = await _service.ObtenerPorIdOptimizado(id, cancellationToken);

            return this.CreatedResponse(
                nameof(GetById),
                new { id },
                result,
                "Almacén creado exitosamente");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarAlmacenDto dto, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<ActualizarAlmacenCommand>(dto);
            command = command with { Id = id };
            await _mediator.Send(command, cancellationToken);
            var result = await _service.ObtenerPorIdOptimizado(id, cancellationToken);

            return this.OkResponse(result, "Almacén actualizado correctamente");
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            await _mediator.Send(new ActualizarEstadoAlmacenCommand(id, true));
            return this.OkResponse(string.Empty, "Almacén activado correctamente");
        }

        [HttpPatch("{id}/inactivar")]
        public async Task<IActionResult> Inactivar(int id)
        {
            await _mediator.Send(new ActualizarEstadoAlmacenCommand(id, false));
            return this.OkResponse(string.Empty, "Almacén inactivado correctamente");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new EliminarAlmacenCommand(id));
            return this.OkResponse(string.Empty, "Almacén eliminado correctamente");
        }
    }
}
