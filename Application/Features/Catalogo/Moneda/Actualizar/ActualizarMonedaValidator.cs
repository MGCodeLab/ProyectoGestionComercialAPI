using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.Moneda.Actualizar;

public class ActualizarMonedaValidator : AbstractValidator<ActualizarMonedaCommand>
{
    private readonly IMonedaValidatorService _validatorService;

    public ActualizarMonedaValidator(IMonedaValidatorService validatorService)
    {
        _validatorService = validatorService;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID es requerido");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Simbolo)
            .NotEmpty().WithMessage("El símbolo es requerido")
            .MaximumLength(5).WithMessage("El símbolo no puede exceder 5 caracteres");

        RuleFor(x => x.CodigoISO)
            .NotEmpty().WithMessage("El código ISO es requerido")
            .Length(3).WithMessage("El código ISO debe ser exactamente 3 caracteres")
            .MustAsync((cmd, codigoISO, ct) => ValidateCodigoISO(cmd.Id, codigoISO, ct))
            .WithMessage("El código ISO de la moneda ya existe");
    }

    private async Task<bool> ValidateCodigoISO(int monedaId, string codigoISO, CancellationToken cancellationToken)
    {
        return await _validatorService.IsCodigoISOUniqueExcept(monedaId, codigoISO, cancellationToken);
    }
}
