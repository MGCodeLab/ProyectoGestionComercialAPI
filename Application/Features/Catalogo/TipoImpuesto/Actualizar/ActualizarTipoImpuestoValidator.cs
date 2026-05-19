using Application.Features.Catalogo.TipoImpuesto.Actualizar;
using FluentValidation;

namespace Application.Features.Catalogo.TipoImpuesto.Actualizar
{
    public class ActualizarTipoImpuestoValidator : AbstractValidator<ActualizarTipoImpuestoCommand>
    {
        public ActualizarTipoImpuestoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID debe ser mayor a 0");

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
