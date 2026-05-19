namespace Application.Interfaces
{
    public interface IAlmacenValidatorService
    {
        Task<bool> IsCodigoUnique(string codigo, CancellationToken ct);
    }
}
