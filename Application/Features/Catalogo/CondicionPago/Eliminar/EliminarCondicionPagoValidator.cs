using FluentValidation;

namespace Application.Features.Catalogo.CondicionPago.Eliminar;

public class EliminarCondicionPagoValidator : AbstractValidator<EliminarCondicionPagoCommand>
{
    public EliminarCondicionPagoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
