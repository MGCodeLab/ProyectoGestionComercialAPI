using API.GestionComercial.Extensions;
using Application.Dtos.Organizacion;
using Application.Features.Organizacion.Sucursal.Actualizar;
using Application.Features.Organizacion.Sucursal.ActualizarEstado;
using Application.Features.Organizacion.Sucursal.Crear;
using Application.Features.Organizacion.Sucursal.Eliminar;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.GestionComercial.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SucursalesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISucursalService _service;
        private readonly IMediator _mediator;

        public SucursalesController(
            IMapper mapper,
            ISucursalService service,
            IMediator mediator)
        {
            _mapper = mapper;
            _service = service;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var sucursales = await _service.ObtenerTodosOptimizado();
            return this.OkResponse(sucursales);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sucursal = await _service.ObtenerPorIdOptimizado(id);

            if (sucursal == null)
                return this.NotFoundResponse("Sucursal no encontrada");

            return this.OkResponse(sucursal);
        }

        [HttpGet("combo/list")]
        public async Task<IActionResult> GetCombo()
        {
            var result = await _service.ObtenerCombo();
            return this.OkResponse(result, "Sucursales para combo obtenidas exitosamente");
        }

        [HttpGet("combo/list/{idEmpresa}")]
        public async Task<IActionResult> GetComboByIdEmpresa(int idEmpresa)
        {
            var result = await _service.ObtenerComboByIdEmpresa(idEmpresa, HttpContext.RequestAborted);
            return this.OkResponse(result, "Sucursales para combo obtenidas exitosamente");
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearSucursalDto dto)
        {
            var command = _mapper.Map<CrearSucursalCommand>(dto);
            var id = await _mediator.Send(command);
            var result = await _service.ObtenerPorIdOptimizado(id);

            return this.CreatedResponse(
                nameof(GetById),
                new { id },
                result,
                "Sucursal creada exitosamente");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarSucursalDto dto)
        {
            var command = _mapper.Map<ActualizarSucursalCommand>(dto);
            command = command with { Id = id };
            await _mediator.Send(command);
            var result = await _service.ObtenerPorIdOptimizado(id);

            return this.OkResponse(result, "Sucursal actualizada correctamente");
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            await _mediator.Send(new ActualizarEstadoSucursalCommand(id, true));
            return this.OkResponse(string.Empty, "Sucursal activada correctamente");
        }

        [HttpPatch("{id}/inactivar")]
        public async Task<IActionResult> Inactivar(int id)
        {
            await _mediator.Send(new ActualizarEstadoSucursalCommand(id, false));
            return this.OkResponse(string.Empty, "Sucursal inactivada correctamente");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new EliminarSucursalCommand(id));
            return this.OkResponse(string.Empty, "Sucursal eliminada correctamente");
        }
    }
}
