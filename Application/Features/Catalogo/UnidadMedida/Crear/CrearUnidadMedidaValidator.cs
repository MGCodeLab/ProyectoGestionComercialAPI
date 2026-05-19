using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.UnidadMedida.Crear;

public class CrearUnidadMedidaValidator : AbstractValidator<CrearUnidadMedidaCommand>
{
    private readonly IUnidadMedidaValidatorService _validatorService;

    public CrearUnidadMedidaValidator(IUnidadMedidaValidatorService validatorService)
    {
        _validatorService = validatorService;

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
            .MustAsync(BeUniqueCode)
            .WithMessage("El código de unidad de medida ya existe");
    }

    private async Task<bool> BeUniqueCode(string codigo, CancellationToken cancellationToken)
    {
        return await _validatorService.IsCodigoUnique(codigo, cancellationToken);
    }
}
