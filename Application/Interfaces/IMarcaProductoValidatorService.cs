namespace Application.Interfaces
{
    public interface IMarcaProductoValidatorService
    {
        Task<bool> NombreUnicoAsync(string nombre, int? excludeId = null);
    }
}
