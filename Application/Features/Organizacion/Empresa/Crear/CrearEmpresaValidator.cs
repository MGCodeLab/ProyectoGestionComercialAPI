using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Organizacion.Empresa.Crear
{
    public class CrearEmpresaValidator : AbstractValidator<CrearEmpresaCommand>
    {
        private readonly IEmpresaValidatorService _validator;

        public CrearEmpresaValidator(IEmpresaValidatorService validator)
        {
            _validator = validator;

            RuleFor(x => x.RazonSocial)
                .NotEmpty().WithMessage("Razón social requerida")
                .MaximumLength(200).WithMessage("Razón social no debe exceder 200 caracteres");

            RuleFor(x => x.NumeroDocumento)
                .NotEmpty().WithMessage("Número documento requerido")
                .MustAsync(BeUniqueDocumento).WithMessage("Número documento ya existe");

            RuleFor(x => x.TipoDocumentoId)
                .GreaterThan(0).WithMessage("Tipo documento requerido");

            RuleFor(x => x.PaisId)
                .GreaterThan(0).WithMessage("País requerido");

            RuleFor(x => x.MonedaBaseId)
                .GreaterThan(0).WithMessage("Moneda base requerida");
        }

        private async Task<bool> BeUniqueDocumento(string numDoc, CancellationToken ct)
            => await _validator.IsNumeroDocumentoUnique(numDoc, ct);
    }
}
