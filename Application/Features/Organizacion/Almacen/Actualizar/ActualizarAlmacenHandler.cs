using MediatR;
using AutoMapper;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Almacen.Actualizar
{
    public class ActualizarAlmacenHandler : IRequestHandler<ActualizarAlmacenCommand, int>
    {
        private readonly IAlmacenService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ActualizarAlmacenHandler> _logger;

        public ActualizarAlmacenHandler(IAlmacenService service, IMapper mapper, ILogger<ActualizarAlmacenHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(ActualizarAlmacenCommand request, CancellationToken ct)
        {
            var almacen = await _service.ObtenerPorId(request.Id, true);
            if (almacen == null)
                throw new KeyNotFoundException($"Almacén con Id {request.Id} no encontrado");

            _mapper.Map(request, almacen);
            await _service.Actualizar(almacen);

            _logger.LogInformation($"Almacén actualizado: {almacen.Id}");

            return almacen.Id;
        }
    }
}
