using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Organizacion.Almacen.Crear
{
    public class CrearAlmacenValidator : AbstractValidator<CrearAlmacenCommand>
    {
        private readonly IAlmacenValidatorService _validator;

        public CrearAlmacenValidator(IAlmacenValidatorService validator)
        {
            _validator = validator;

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Nombre requerido")
                .MaximumLength(150).WithMessage("Nombre no debe exceder 150 caracteres");

            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("Código requerido")
                .MaximumLength(10).WithMessage("Código no debe exceder 10 caracteres")
                .MustAsync(BeUniqueCodigo).WithMessage("Código ya existe");

            RuleFor(x => x.SucursalId)
                .GreaterThan(0).WithMessage("Sucursal requerida");
        }

        private async Task<bool> BeUniqueCodigo(string codigo, CancellationToken ct)
            => await _validator.IsCodigoUnique(codigo, ct);
    }
}
