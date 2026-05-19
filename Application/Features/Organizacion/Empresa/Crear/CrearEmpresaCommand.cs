using MediatR;

namespace Application.Features.Organizacion.Empresa.Crear
{
    public record CrearEmpresaCommand(
        string RazonSocial,
        string? NombreComercial,
        string NumeroDocumento,
        int TipoDocumentoId,
        int PaisId,
        int MonedaBaseId,
        string? DireccionFiscal,
        string? Telefono,
        string? Correo,
        string? LogoUrl
    ) : IRequest<int>;
}
