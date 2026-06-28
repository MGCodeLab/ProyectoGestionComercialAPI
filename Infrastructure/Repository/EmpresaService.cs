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

        public async Task<Empresa?> ObtenerPorId(int id, bool tracking, CancellationToken cancellationToken)
            => tracking
                ? await _context.Empresas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                : await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<Empresa?> ObtenerPrimera(CancellationToken cancellationToken)
            => await _context.Empresas.FirstOrDefaultAsync(cancellationToken);

        public async Task<List<Empresa>> ObtenerTodos(CancellationToken cancellationToken)
            => await _context.Empresas.ToListAsync(cancellationToken);

        public async Task<List<ComboDto>> ObtenerCombo(CancellationToken cancellationToken)
            => await _context.Empresas
                .AsNoTracking()
                .Where(e => e.Activo)
                .Select(e => new ComboDto
                {
                    Id = e.Id,
                    Nombre = e.RazonSocial
                })
                .ToListAsync(cancellationToken);

        public async Task<int> Crear(Empresa empresa, CancellationToken cancellationToken)
        {
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync(cancellationToken);
            return empresa.Id;
        }

        public async Task Actualizar(Empresa empresa, CancellationToken cancellationToken)
        {
            empresa.FechaActualizacion = DateTime.UtcNow;
            _context.Empresas.Update(empresa);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> TieneDependencias(Empresa entity, CancellationToken token)
        {
            var existeSucursal = await _context.Sucursales
                .AsNoTracking()
                .AnyAsync(e => e.EmpresaId == entity.Id, token);

            return existeSucursal;
        }

        public async Task Eliminar(int id, CancellationToken cancellationToken)
        {
            var empresa = await _context.Empresas.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
            if (empresa != null)
            {
                _context.Empresas.Remove(empresa);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
