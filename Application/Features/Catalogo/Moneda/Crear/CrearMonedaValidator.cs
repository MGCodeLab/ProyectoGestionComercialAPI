using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.Moneda.Crear;

public class CrearMonedaValidator : AbstractValidator<CrearMonedaCommand>
{
    private readonly IMonedaValidatorService _validatorService;

    public CrearMonedaValidator(IMonedaValidatorService validatorService)
    {
        _validatorService = validatorService;

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Simbolo)
            .NotEmpty().WithMessage("El símbolo es requerido")
            .MaximumLength(5).WithMessage("El símbolo no puede exceder 5 caracteres");

        RuleFor(x => x.CodigoISO)
            .NotEmpty().WithMessage("El código ISO es requerido")
            .Length(3).WithMessage("El código ISO debe ser exactamente 3 caracteres")
            .MustAsync(BeUniqueCodigoISO).WithMessage("El código ISO de la moneda ya existe");
    }

    private async Task<bool> BeUniqueCodigoISO(string codigoISO, CancellationToken cancellationToken)
    {
        return await _validatorService.IsCodigoISOUnique(codigoISO, cancellationToken);
    }
}
