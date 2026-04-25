using FluentValidation;

namespace Application.Features.Clientes.Crear
{
    public class CrearClienteValidator : AbstractValidator<CrearClienteCommand>
    {
        public CrearClienteValidator()
        {
            RuleFor(x => x.TipoDocumentoId)
                .GreaterThan(0)
                .WithMessage("TipoDocumento es requerido");

            RuleFor(x => x.NumeroDocumento)
                .NotEmpty()
                .WithMessage("Número de documento es requerido")
                .MaximumLength(20)
                .WithMessage("Número de documento no puede exceder 20 caracteres");

            RuleFor(x => x.Nombres)
                .NotEmpty()
                .WithMessage("Nombres es requerido")
                .MaximumLength(100)
                .WithMessage("Nombres no puede exceder 100 caracteres");

            RuleFor(x => x.ApellidoPaterno)
                .NotEmpty()
                .WithMessage("Apellido paterno es requerido")
                .MaximumLength(100)
                .WithMessage("Apellido paterno no puede exceder 100 caracteres");

            RuleFor(x => x.ApellidoMaterno)
                .MaximumLength(100)
                .WithMessage("Apellido materno no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.ApellidoMaterno));

            RuleFor(x => x.Correo)
                .EmailAddress()
                .WithMessage("Correo debe ser una dirección de correo válida")
                .MaximumLength(150)
                .WithMessage("Correo no puede exceder 150 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Correo));

            RuleFor(x => x.Telefono)
                .MaximumLength(20)
                .WithMessage("Teléfono no puede exceder 20 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Telefono));

            RuleFor(x => x.Direccion)
                .MaximumLength(250)
                .WithMessage("Dirección no puede exceder 250 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Direccion));
        }
    }
}
