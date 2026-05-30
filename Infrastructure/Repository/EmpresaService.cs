using Application.Dtos;
using Application.Interfaces;
using Domain.Catalogo;
using Domain.Organizacion;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class EmpresaService : IEmpresaService
    {
        private readonly AppDbContext _context;

        public EmpresaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Empresa?> ObtenerPorId(int id, bool tracking = false)
            => tracking
                ? await _context.Empresas.FirstOrDefaultAsync(x => x.Id == id)
                : await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Empresa?> ObtenerPrimera()
            => await _context.Empresas.FirstOrDefaultAsync();

        public async Task<List<Empresa>> ObtenerTodos()
            => await _context.Empresas.ToListAsync();

        public async Task<List<ComboDto>> ObtenerCombo()
            => await _context.Empresas
                .AsNoTracking()
                .Where(e => e.Activo)
                .Select(e => new ComboDto
                {
                    Id = e.Id,
                    Nombre = e.RazonSocial
                })
                .ToListAsync();

        public async Task<int> Crear(Empresa empresa)
        {
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
            return empresa.Id;
        }

        public async Task Actualizar(Empresa empresa)
        {
            empresa.FechaActualizacion = DateTime.UtcNow;
            _context.Empresas.Update(empresa);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TieneDependencias(Empresa entity, CancellationToken token)
        {
            var existeSucursal = await _context.Sucursales
                .AsNoTracking()
                .AnyAsync(e => e.EmpresaId == entity.Id, token);

            return existeSucursal;
        }

        public async Task Eliminar(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa != null)
            {
                _context.Empresas.Remove(empresa);
                await _context.SaveChangesAsync();
            }
        }
    }
}
