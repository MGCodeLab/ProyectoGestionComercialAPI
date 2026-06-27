using FluentValidation;

namespace Application.Features.Catalogo.MarcaProducto.ActualizarEstado.MarcaProducto;

public class ActualizarEstadoMarcaProductoValidator : AbstractValidator<ActualizarEstadoMarcaProductoCommand>
{
    public ActualizarEstadoMarcaProductoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
