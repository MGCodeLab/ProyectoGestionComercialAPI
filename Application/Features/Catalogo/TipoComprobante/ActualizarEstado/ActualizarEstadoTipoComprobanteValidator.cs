using FluentValidation;

namespace Application.Features.Catalogo.TipoComprobante.ActualizarEstado.TipoComprobante;

public class ActualizarEstadoTipoComprobanteValidator : AbstractValidator<ActualizarEstadoTipoComprobanteCommand>
{
    public ActualizarEstadoTipoComprobanteValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
