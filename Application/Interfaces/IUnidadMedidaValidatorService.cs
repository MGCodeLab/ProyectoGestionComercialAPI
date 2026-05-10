namespace Application.Interfaces;

public interface IUnidadMedidaValidatorService
{
    Task<bool> IsCodigoUnique(string codigo, CancellationToken cancellationToken);
    Task<bool> IsCodigoUniqueExcept(int unidadId, string codigo, CancellationToken cancellationToken);
}
