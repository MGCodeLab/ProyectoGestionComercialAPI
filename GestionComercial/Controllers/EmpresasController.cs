using Application.Dtos.Organizacion;
using Application.Features.Organizacion.Empresa.Crear;
using Application.Features.Organizacion.Empresa.Actualizar;
using Application.Features.Organizacion.Empresa.ActualizarEstado;
using Application.Features.Organizacion.Empresa.Eliminar;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.GestionComercial.Extensions;

namespace API.GestionComercial.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmpresasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEmpresaService _service;
        private readonly IMediator _mediator;

        public EmpresasController(
            IMapper mapper,
            IEmpresaService service,
            IMediator mediator)
        {
            _mapper = mapper;
            _service = service;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var empresas = await _service.ObtenerTodos();
            var result = _mapper.Map<List<EmpresaDto>>(empresas);
            return this.OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresa = await _service.ObtenerPorId(id, tracking: false);

            if (empresa == null)
                return this.NotFoundResponse("Empresa no encontrada");

            var result = _mapper.Map<EmpresaDto>(empresa);
            return this.OkResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearEmpresaDto dto)
        {
            var command = _mapper.Map<CrearEmpresaCommand>(dto);
            var id = await _mediator.Send(command);

            var result = new { id, razonSocial = dto.RazonSocial };
            return this.CreatedResponse(
                nameof(GetById),
                new { id },
                result,
                "Empresa creada exitosamente");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarEmpresaDto dto)
        {
            var command = _mapper.Map<ActualizarEmpresaCommand>(dto);
            command = command with { Id = id };

            await _mediator.Send(command);

            return this.OkResponse(string.Empty, "Empresa actualizada correctamente");
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            await _mediator.Send(new ActualizarEstadoEmpresaCommand(id, true));
            return this.OkResponse(string.Empty, "Empresa activada correctamente");
        }

        [HttpPatch("{id}/inactivar")]
        public async Task<IActionResult> Inactivar(int id)
        {
            await _mediator.Send(new ActualizarEstadoEmpresaCommand(id, false));
            return this.OkResponse(string.Empty, "Empresa inactivada correctamente");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new EliminarEmpresaCommand(id));
            return this.OkResponse(string.Empty, "Empresa eliminada correctamente");
        }
    }
}
