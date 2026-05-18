using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class CategoriaProductoValidatorService : ICategoriaProductoValidatorService
    {
        private readonly AppDbContext _context;

        public CategoriaProductoValidatorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoriaProducto?> ObtenerPorIdAsync(int id)
        {
            return await _context.CategoriasProducto.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> CalcularProfundidadAsync(int categoriaId)
        {
            var categoria = await _context.CategoriasProducto.FindAsync(categoriaId);
            if (categoria?.CategoriaPadreId == null)
                return 1;

            var profundidad = 1;
            var padreId = categoria.CategoriaPadreId;

            while (padreId.HasValue)
            {
                var padre = await _context.CategoriasProducto.FindAsync(padreId);
                if (padre == null)
                    break;

                profundidad++;
                padreId = padre.CategoriaPadreId;
            }

            return profundidad;
        }

        public async Task<bool> EsDescendienteDeAsync(int ancestorId, int descendantId)
        {
            var actual = await _context.CategoriasProducto.FindAsync(descendantId);

            while (actual?.CategoriaPadreId.HasValue == true)
            {
                if (actual.CategoriaPadreId == ancestorId)
                    return true;
                actual = await _context.CategoriasProducto.FindAsync(actual.CategoriaPadreId);
            }

            return false;
        }
    }
}
