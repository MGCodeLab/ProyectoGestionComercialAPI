using FluentValidation;

namespace Application.Features.Catalogo.TipoDocumento.ActualizarEstado;

public class ActualizarEstadoTipoDocumentoValidator : AbstractValidator<ActualizarEstadoTipoDocumentoCommand>
{
    public ActualizarEstadoTipoDocumentoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
