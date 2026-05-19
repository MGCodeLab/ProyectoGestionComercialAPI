using Application.Features.Catalogo.TipoComprobante.Crear;
using FluentValidation;

namespace Application.Features.Catalogo.TipoComprobante.Crear
{
    public class CrearTipoComprobanteValidator : AbstractValidator<CrearTipoComprobanteCommand>
    {
        public CrearTipoComprobanteValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Nombre es requerido")
                .MaximumLength(100).WithMessage("Nombre no puede exceder 100 caracteres");

            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("Código es requerido")
                .MaximumLength(5).WithMessage("Código no puede exceder 5 caracteres");

            RuleFor(x => x.AfectaInventario)
                .NotNull().WithMessage("AfectaInventario es requerido");

            RuleFor(x => x.AfectaContable)
                .NotNull().WithMessage("AfectaContable es requerido");
        }
    }
}
