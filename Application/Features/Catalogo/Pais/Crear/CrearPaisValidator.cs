using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.Pais.Crear;

public class CrearPaisValidator : AbstractValidator<CrearPaisCommand>
{
    private readonly IPaisValidatorService _validatorService;

    public CrearPaisValidator(IPaisValidatorService validatorService)
    {
        _validatorService = validatorService;

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .Length(2).WithMessage("El código debe ser exactamente 2 caracteres")
            .MustAsync(BeUniqueCode).WithMessage("El código del país ya existe");

        RuleFor(x => x.CodigoMoneda)
            .NotEmpty().WithMessage("El código de moneda es requerido")
            .Length(3).WithMessage("El código de moneda debe ser exactamente 3 caracteres");
    }

    private async Task<bool> BeUniqueCode(string codigo, CancellationToken cancellationToken)
    {
        return await _validatorService.IsCodigoUnique(codigo, cancellationToken);
    }
}
