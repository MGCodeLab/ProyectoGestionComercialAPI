using FluentValidation;

namespace Application.Features.Catalogo.SerieDocumento.Eliminar;

public class EliminarSerieDocumentoValidator : AbstractValidator<EliminarSerieDocumentoCommand>
{
    public EliminarSerieDocumentoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
