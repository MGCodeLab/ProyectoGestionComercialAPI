using Application.Features.Catalogo.TipoComprobante.Actualizar;
using FluentValidation;

namespace Application.Features.Catalogo.TipoComprobante.Actualizar
{
    public class ActualizarTipoComprobanteValidator : AbstractValidator<ActualizarTipoComprobanteCommand>
    {
        public ActualizarTipoComprobanteValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID debe ser mayor a 0");

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
