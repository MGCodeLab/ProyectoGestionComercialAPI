using FluentValidation;

namespace Application.Features.Catalogo.ListaPrecio.Crear;

public class CrearListaPrecioValidator : AbstractValidator<CrearListaPrecioCommand>
{
    public CrearListaPrecioValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(150).WithMessage("El nombre no puede exceder 150 caracteres");

        RuleFor(x => x.MonedaId)
            .GreaterThan(0).WithMessage("El ID de moneda es requerido");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));
    }
}
