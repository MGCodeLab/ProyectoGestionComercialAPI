namespace Application.Interfaces
{
    public interface IEmpresaValidatorService
    {
        Task<bool> IsNumeroDocumentoUnique(string numeroDocumento, CancellationToken ct);
    }
}
