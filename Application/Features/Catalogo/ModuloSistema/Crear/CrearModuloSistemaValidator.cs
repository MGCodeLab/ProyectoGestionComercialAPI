using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.ModuloSistema.Crear;

public class CrearModuloSistemaValidator : AbstractValidator<CrearModuloSistemaCommand>
{
    private readonly IModuloSistemaValidatorService _validatorService;

    public CrearModuloSistemaValidator(IModuloSistemaValidatorService validatorService)
    {
        _validatorService = validatorService;

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
            .MustAsync(BeUniqueCode)
            .WithMessage("El código del módulo ya existe");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .WithMessage("Máximo 500 caracteres en descripción");
    }

    private async Task<bool> BeUniqueCode(string codigo, CancellationToken cancellationToken)
    {
        return await _validatorService.IsCodigoUnique(codigo, cancellationToken);
    }
}
