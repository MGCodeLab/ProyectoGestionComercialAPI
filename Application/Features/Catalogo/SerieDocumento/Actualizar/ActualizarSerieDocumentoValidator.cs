using Application.Features.Catalogo.SerieDocumento.Actualizar;
using FluentValidation;

namespace Application.Features.Catalogo.SerieDocumento.Actualizar
{
    public class ActualizarSerieDocumentoValidator : AbstractValidator<ActualizarSerieDocumentoCommand>
    {
        public ActualizarSerieDocumentoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID debe ser mayor a 0");

            RuleFor(x => x.TipoComprobanteId)
                .GreaterThan(0).WithMessage("TipoComprobanteId debe ser mayor a 0");

            RuleFor(x => x.SucursalId)
                .GreaterThan(0).WithMessage("SucursalId debe ser mayor a 0");

            RuleFor(x => x.Serie)
                .NotEmpty().WithMessage("Serie es requerida")
                .MaximumLength(4).WithMessage("Serie no puede exceder 4 caracteres");

            RuleFor(x => x.NumeroMaximo)
                .GreaterThan(0).WithMessage("NumeroMaximo debe ser mayor a 0")
                .When(x => x.NumeroMaximo.HasValue);
        }
    }
}
