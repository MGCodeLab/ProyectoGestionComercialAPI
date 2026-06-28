using FluentValidation;

namespace Application.Features.Catalogo.SerieDocumento.ActualizarEstado;

public class ActualizarEstadoSerieDocumentoValidator : AbstractValidator<ActualizarEstadoSerieDocumentoCommand>
{
    public ActualizarEstadoSerieDocumentoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
