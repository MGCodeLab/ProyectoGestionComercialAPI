using Application.Features.Catalogo.TipoImpuesto.Crear;
using FluentValidation;

namespace Application.Features.Catalogo.TipoImpuesto.Crear
{
    public class CrearTipoImpuestoValidator : AbstractValidator<CrearTipoImpuestoCommand>
    {
        public CrearTipoImpuestoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Nombre es requerido")
                .MaximumLength(100).WithMessage("Nombre no puede exceder 100 caracteres");

            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("Código es requerido")
                .MaximumLength(10).WithMessage("Código no puede exceder 10 caracteres");

            RuleFor(x => x.Porcentaje)
                .GreaterThanOrEqualTo(0).WithMessage("Porcentaje no puede ser negativo")
                .LessThanOrEqualTo(100).WithMessage("Porcentaje no puede exceder 100");

            RuleFor(x => x.EsIncluido)
                .NotNull().WithMessage("EsIncluido es requerido");
        }
    }
}
