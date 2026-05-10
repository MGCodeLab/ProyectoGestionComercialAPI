namespace Application.Interfaces;

public interface IModuloSistemaValidatorService
{
    Task<bool> IsCodigoUnique(string codigo, CancellationToken cancellationToken);
    Task<bool> IsCodigoUniqueExcept(int moduloId, string codigo, CancellationToken cancellationToken);
}
