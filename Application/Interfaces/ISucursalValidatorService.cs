namespace Application.Interfaces
{
    public interface ISucursalValidatorService
    {
        Task<bool> IsCodigoUnique(string codigo, CancellationToken ct);
    }
}
