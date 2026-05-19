using FluentValidation;

namespace Application.Features.Organizacion.Sucursal.Actualizar
{
    public class ActualizarSucursalValidator : AbstractValidator<ActualizarSucursalCommand>
    {
        public ActualizarSucursalValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id requerido");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Nombre requerido")
                .MaximumLength(150).WithMessage("Nombre no debe exceder 150 caracteres");

            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("Código requerido")
                .MaximumLength(10).WithMessage("Código no debe exceder 10 caracteres");

            RuleFor(x => x.EmpresaId)
                .GreaterThan(0).WithMessage("Empresa requerida");

            RuleFor(x => x.PaisId)
                .GreaterThan(0).WithMessage("País requerido");
        }
    }
}
