using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.Pais.Actualizar;

public class ActualizarPaisValidator : AbstractValidator<ActualizarPaisCommand>
{
    private readonly IPaisValidatorService _validatorService;

    public ActualizarPaisValidator(IPaisValidatorService validatorService)
    {
        _validatorService = validatorService;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID es requerido");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .Length(2).WithMessage("El código debe ser exactamente 2 caracteres")
            .MustAsync((cmd, codigo, ct) => BeUniquePaisCode(cmd.Id, codigo, ct))
            .WithMessage("El código del país ya existe");

        RuleFor(x => x.CodigoMoneda)
            .NotEmpty().WithMessage("El código de moneda es requerido")
            .Length(3).WithMessage("El código de moneda debe ser exactamente 3 caracteres");
    }

    private async Task<bool> BeUniquePaisCode(int paisId, string codigo, CancellationToken cancellationToken)
    {
        return await _validatorService.IsCodigoUniqueExcept(paisId, codigo, cancellationToken);
    }
}
