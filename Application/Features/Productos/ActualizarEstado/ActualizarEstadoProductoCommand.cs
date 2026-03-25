using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Productos.ActualizarEstado
{
    public record ActualizarEstadoProductoCommand(int Id, bool Activo) : IRequest<Unit>;
}
