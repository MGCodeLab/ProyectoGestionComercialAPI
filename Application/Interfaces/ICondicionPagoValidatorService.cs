namespace Application.Interfaces;

public interface ICondicionPagoValidatorService
{
    Task<bool> NombreUnicoAsync(string nombre, int? excludeId = null);
}
