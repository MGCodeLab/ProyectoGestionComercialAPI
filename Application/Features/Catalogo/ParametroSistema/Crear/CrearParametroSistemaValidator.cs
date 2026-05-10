using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.ParametroSistema.Crear;

public class CrearParametroSistemaValidator : AbstractValidator<CrearParametroSistemaCommand>
{
    private readonly IParametroSistemaValidatorService _validatorService;

    public CrearParametroSistemaValidator(IParametroSistemaValidatorService validatorService)
    {
        _validatorService = validatorService;

        RuleFor(x => x.Clave)
            .NotEmpty()
            .WithMessage("La clave es requerida")
            .MaximumLength(100)
            .WithMessage("Máximo 100 caracteres en clave")
            .MustAsync(BeUniqueClave)
            .WithMessage("La clave del parámetro ya existe");

        RuleFor(x => x.Valor)
            .NotEmpty()
            .WithMessage("El valor es requerido")
            .MaximumLength(500)
            .WithMessage("Máximo 500 caracteres en valor");

        RuleFor(x => x.TipoDato)
            .NotEmpty()
            .WithMessage("El tipo de dato es requerido")
            .MaximumLength(20)
            .WithMessage("Máximo 20 caracteres en tipo de dato");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .WithMessage("Máximo 500 caracteres en descripción");
    }

    private async Task<bool> BeUniqueClave(string clave, CancellationToken cancellationToken)
    {
        return await _validatorService.IsClaveUnique(clave, cancellationToken);
    }
}
