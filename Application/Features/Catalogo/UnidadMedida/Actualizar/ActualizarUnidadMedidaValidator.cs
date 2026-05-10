using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.UnidadMedida.Actualizar;

public class ActualizarUnidadMedidaValidator : AbstractValidator<ActualizarUnidadMedidaCommand>
{
    private readonly IUnidadMedidaValidatorService _validatorService;

    public ActualizarUnidadMedidaValidator(IUnidadMedidaValidatorService validatorService)
    {
        _validatorService = validatorService;

        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El id debe ser mayor a 0");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es requerido")
            .MaximumLength(100)
            .WithMessage("Máximo 100 caracteres en nombre");

        RuleFor(x => x.Simbolo)
            .NotEmpty()
            .WithMessage("El símbolo es requerido")
            .MaximumLength(10)
            .WithMessage("Máximo 10 caracteres en símbolo");

        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage("El código es requerido")
            .MaximumLength(10)
            .WithMessage("Máximo 10 caracteres en código")
            .MustAsync(BeUniqueCodeExcept)
            .WithMessage("El código de unidad de medida ya existe");
    }

    private async Task<bool> BeUniqueCodeExcept(ActualizarUnidadMedidaCommand command, string codigo, CancellationToken cancellationToken)
    {
        return await _validatorService.IsCodigoUniqueExcept(command.Id, codigo, cancellationToken);
    }
}
