namespace Application.Interfaces;

public interface IListaPrecioValidatorService
{
    Task<bool> NombreUnicoAsync(string nombre, int? excludeId = null);
    Task<bool> ExisteDefaultAsync(int? excludeId = null);
    Task<bool> MonedaExisteAsync(int monedaId);
}
