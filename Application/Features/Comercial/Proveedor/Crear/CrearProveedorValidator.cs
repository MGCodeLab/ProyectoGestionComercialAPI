using FluentValidation;

namespace Application.Features.Comercial.Proveedor.Crear;

public class CrearProveedorValidator : AbstractValidator<CrearProveedorCommand>
{
    public CrearProveedorValidator()
    {
        RuleFor(x => x.TipoDocumentoId)
            .GreaterThan(0).WithMessage("El tipo de documento es requerido");

        RuleFor(x => x.NumeroDocumento)
            .NotEmpty().WithMessage("El número de documento es requerido")
            .MaximumLength(20).WithMessage("El número de documento no puede exceder 20 caracteres");

        RuleFor(x => x.RazonSocial)
            .NotEmpty().WithMessage("La razón social es requerida")
            .MaximumLength(200).WithMessage("La razón social no puede exceder 200 caracteres");

        RuleFor(x => x.NombreComercial)
            .MaximumLength(150).WithMessage("El nombre comercial no puede exceder 150 caracteres")
            .When(x => !string.IsNullOrEmpty(x.NombreComercial));

        RuleFor(x => x.PaisId)
            .GreaterThan(0).WithMessage("El país es requerido");

        RuleFor(x => x.Correo)
            .EmailAddress().WithMessage("El correo debe ser válido")
            .MaximumLength(150).WithMessage("El correo no puede exceder 150 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Correo));

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono));

        RuleFor(x => x.Direccion)
            .MaximumLength(300).WithMessage("La dirección no puede exceder 300 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Direccion));
    }
}
