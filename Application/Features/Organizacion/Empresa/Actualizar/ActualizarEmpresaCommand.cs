using MediatR;

namespace Application.Features.Organizacion.Empresa.Actualizar
{
    public record ActualizarEmpresaCommand(
        string RazonSocial,
        string? NombreComercial,
        string NumeroDocumento,
        int TipoDocumentoId,
        int PaisId,
        int MonedaBaseId,
        string? DireccionFiscal,
        string? Telefono,
        string? Correo,
        string? LogoUrl,
        int Id = 0
    ) : IRequest<int>;
}
