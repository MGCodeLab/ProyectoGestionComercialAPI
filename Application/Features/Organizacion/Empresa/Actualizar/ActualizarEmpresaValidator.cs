using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Organizacion.Empresa.Actualizar
{
    public class ActualizarEmpresaValidator : AbstractValidator<ActualizarEmpresaCommand>
    {
        private readonly IEmpresaValidatorService _validator;

        public ActualizarEmpresaValidator(IEmpresaValidatorService validator)
        {
            _validator = validator;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id requerido");

            RuleFor(x => x.RazonSocial)
                .NotEmpty().WithMessage("Razón social requerida")
                .MaximumLength(200).WithMessage("Razón social no debe exceder 200 caracteres");

            RuleFor(x => x.NumeroDocumento)
                .NotEmpty().WithMessage("Número documento requerido");

            RuleFor(x => x.TipoDocumentoId)
                .GreaterThan(0).WithMessage("Tipo documento requerido");

            RuleFor(x => x.PaisId)
                .GreaterThan(0).WithMessage("País requerido");

            RuleFor(x => x.MonedaBaseId)
                .GreaterThan(0).WithMessage("Moneda base requerida");
        }
    }
}
