using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Organizacion.Sucursal.Crear
{
    public class CrearSucursalValidator : AbstractValidator<CrearSucursalCommand>
    {
        private readonly ISucursalValidatorService _validator;

        public CrearSucursalValidator(ISucursalValidatorService validator)
        {
            _validator = validator;

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Nombre requerido")
                .MaximumLength(150).WithMessage("Nombre no debe exceder 150 caracteres");

            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("Código requerido")
                .MaximumLength(10).WithMessage("Código no debe exceder 10 caracteres")
                .MustAsync(BeUniqueCodigo).WithMessage("Código ya existe");

            RuleFor(x => x.EmpresaId)
                .GreaterThan(0).WithMessage("Empresa requerida");

            RuleFor(x => x.PaisId)
                .GreaterThan(0).WithMessage("País requerido");
        }

        private async Task<bool> BeUniqueCodigo(string codigo, CancellationToken ct)
            => await _validator.IsCodigoUnique(codigo, ct);
    }
}
