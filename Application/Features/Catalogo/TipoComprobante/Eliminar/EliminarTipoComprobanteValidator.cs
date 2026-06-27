using FluentValidation;

namespace Application.Features.Catalogo.TipoComprobante.Eliminar.TipoComprobante;

public class EliminarTipoComprobanteValidator : AbstractValidator<EliminarTipoComprobanteCommand>
{
    public EliminarTipoComprobanteValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
