using FluentValidation;

namespace Application.Features.Catalogo.TipoDocumento.Crear;

public class CrearTipoDocumentoValidator : AbstractValidator<CrearTipoDocumentoCommand>
{
    public CrearTipoDocumentoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(5).WithMessage("El código no puede exceder 5 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");
    }
}
