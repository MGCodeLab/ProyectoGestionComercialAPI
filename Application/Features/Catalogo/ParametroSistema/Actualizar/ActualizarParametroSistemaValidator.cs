using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.ParametroSistema.Actualizar;

public class ActualizarParametroSistemaValidator : AbstractValidator<ActualizarParametroSistemaCommand>
{
    private readonly IParametroSistemaValidatorService _validatorService;

    public ActualizarParametroSistemaValidator(IParametroSistemaValidatorService validatorService)
    {
        _validatorService = validatorService;

        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El id debe ser mayor a 0");

        RuleFor(x => x.Clave)
            .NotEmpty()
            .WithMessage("La clave es requerida")
            .MaximumLength(100)
            .WithMessage("Máximo 100 caracteres en clave")
            .MustAsync(BeUniqueClaveExcept)
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

    private async Task<bool> BeUniqueClaveExcept(ActualizarParametroSistemaCommand command, string clave, CancellationToken cancellationToken)
    {
        return await _validatorService.IsClaveUniqueExcept(command.Id, clave, cancellationToken);
    }
}
