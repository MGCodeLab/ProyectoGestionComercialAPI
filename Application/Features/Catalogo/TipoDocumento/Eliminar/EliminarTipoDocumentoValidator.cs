using FluentValidation;

namespace Application.Features.Catalogo.TipoDocumento.Eliminar;

public class EliminarTipoDocumentoValidator : AbstractValidator<EliminarTipoDocumentoCommand>
{
    public EliminarTipoDocumentoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
