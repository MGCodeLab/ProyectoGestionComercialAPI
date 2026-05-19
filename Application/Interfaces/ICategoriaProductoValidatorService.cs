using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface ICategoriaProductoValidatorService
    {
        Task<CategoriaProducto?> ObtenerPorIdAsync(int id);
        Task<int> CalcularProfundidadAsync(int categoriaId);
        Task<bool> EsDescendienteDeAsync(int ancestorId, int descendantId);
    }
}
