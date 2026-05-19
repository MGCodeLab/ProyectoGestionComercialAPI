namespace Application.Interfaces;

public interface IMonedaValidatorService
{
    Task<bool> IsCodigoISOUnique(string codigoISO, CancellationToken cancellationToken);
    Task<bool> IsCodigoISOUniqueExcept(int monedaId, string codigoISO, CancellationToken cancellationToken);
}
