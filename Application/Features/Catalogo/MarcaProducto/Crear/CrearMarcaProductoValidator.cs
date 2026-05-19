using Application.Interfaces;
using FluentValidation;

namespace Application.Features.Catalogo.MarcaProducto.Crear
{
    public class CrearMarcaProductoValidator : AbstractValidator<CrearMarcaProductoCommand>
    {
        private readonly IMarcaProductoValidatorService _validator;

        public CrearMarcaProductoValidator(IMarcaProductoValidatorService validator)
        {
            _validator = validator;

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Nombre es requerido")
                .MaximumLength(150).WithMessage("Nombre no puede exceder 150 caracteres")
                .MustAsync(async (nombre, ct) =>
                {
                    var esUnico = await _validator.NombreUnicoAsync(nombre);
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
