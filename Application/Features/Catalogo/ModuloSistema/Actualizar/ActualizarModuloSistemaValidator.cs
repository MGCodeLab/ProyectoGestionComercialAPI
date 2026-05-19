using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.ModuloSistema.Actualizar;

public class ActualizarModuloSistemaValidator : AbstractValidator<ActualizarModuloSistemaCommand>
{
    private readonly IModuloSistemaValidatorService _validatorService;

    public ActualizarModuloSistemaValidator(IModuloSistemaValidatorService validatorService)
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

        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage("El código es requerido")
            .MaximumLength(50)
            .WithMessage("Máximo 50 caracteres en código")
            .MustAsync(BeUniqueCodeExcept)
            .WithMessage("El código del módulo ya existe");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .WithMessage("Máximo 500 caracteres en descripción");
    }

    private async Task<bool> BeUniqueCodeExcept(ActualizarModuloSistemaCommand command, string codigo, CancellationToken cancellationToken)
    {
        return await _validatorService.IsCodigoUniqueExcept(command.Id, codigo, cancellationToken);
    }
}
