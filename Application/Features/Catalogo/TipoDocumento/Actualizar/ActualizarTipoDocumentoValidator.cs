using FluentValidation;

namespace Application.Features.Catalogo.TipoDocumento.Actualizar;

public class ActualizarTipoDocumentoValidator : AbstractValidator<ActualizarTipoDocumentoCommand>
{
    public ActualizarTipoDocumentoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(5).WithMessage("El código no puede exceder 5 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");
    }
}
