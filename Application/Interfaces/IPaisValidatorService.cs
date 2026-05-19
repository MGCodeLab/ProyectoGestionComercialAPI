namespace Application.Interfaces;

public interface IPaisValidatorService
{
    Task<bool> IsCodigoUnique(string codigo, CancellationToken cancellationToken);
    Task<bool> IsCodigoUniqueExcept(int paisId, string codigo, CancellationToken cancellationToken);
}
