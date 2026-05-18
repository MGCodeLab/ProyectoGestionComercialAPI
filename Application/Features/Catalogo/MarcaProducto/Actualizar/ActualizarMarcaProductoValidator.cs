using Application.Interfaces;
using FluentValidation;

namespace Application.Features.Catalogo.MarcaProducto.Actualizar
{
    public class ActualizarMarcaProductoValidator : AbstractValidator<ActualizarMarcaProductoCommand>
    {
        private readonly IMarcaProductoValidatorService _validator;

        public ActualizarMarcaProductoValidator(IMarcaProductoValidatorService validator)
        {
            _validator = validator;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID debe ser mayor a 0");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Nombre es requerido")
                .MaximumLength(150).WithMessage("Nombre no puede exceder 150 caracteres")
                .MustAsync(async (cmd, nombre, ct) =>
                {
                    var esUnico = await _validator.NombreUnicoAsync(nombre, cmd.Id);
                    return esUnico;
                })
                .WithMessage("Nombre ya existe");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("Descripción no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.LogoUrl)
                .MaximumLength(500).WithMessage("URL del logo no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.LogoUrl));
        }
    }
}
