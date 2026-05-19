using FluentValidation;

namespace Application.Features.Catalogo.CondicionPago.Actualizar;

public class ActualizarCondicionPagoValidator : AbstractValidator<ActualizarCondicionPagoCommand>
{
    public ActualizarCondicionPagoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID debe ser mayor que cero");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.DiasCredito)
            .GreaterThanOrEqualTo(0).WithMessage("Los días de crédito no pueden ser negativos")
            .LessThanOrEqualTo(999).WithMessage("Los días de crédito no pueden exceder 999");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));
    }
}
