using Application.Dtos;
using Application.Dtos.Organizacion;
using Application.Interfaces;
using Domain.Organizacion;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class AlmacenService : IAlmacenService
    {
        private readonly AppDbContext _context;

        public AlmacenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Almacen?> ObtenerPorId(int id, bool tracking, CancellationToken cancellationToken)
            => tracking
                ? await _context.Almacenes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                : await _context.Almacenes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<List<Almacen>> ObtenerTodos(CancellationToken cancellationToken)
            => await _context.Almacenes.ToListAsync(cancellationToken);

        public async Task<List<AlmacenDto>> ObtenerTodosOptimizado(CancellationToken cancellationToken)
            => await _context.Almacenes
                .AsNoTracking()
                .Select(a => new AlmacenDto
                {
                    Id = a.Id,
                    PublicId = a.PublicId,
                    Nombre = a.Nombre,
                    Codigo = a.Codigo,
                    SucursalId = a.SucursalId,
                    Descripcion = a.Descripcion,
                    EsPrincipal = a.EsPrincipal,
                    Activo = a.Activo,
                    FechaRegistro = a.FechaRegistro,
                    FechaActualizacion = a.FechaActualizacion,
                    Sucursal = new SucursalSlimDto
                    {
                        Id = a.Sucursal.Id,
                        Nombre = a.Sucursal.Nombre
                    },
                    Empresa = new EmpresaSlimDto
                    {
                        Id = a.Sucursal.Empresa.Id,
                        RazonSocial = a.Sucursal.Empresa.RazonSocial
                    }
                })
                .ToListAsync(cancellationToken);

        public async Task<AlmacenDto?> ObtenerPorIdOptimizado(int id, CancellationToken cancellationToken)
            => await _context.Almacenes
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(a => new AlmacenDto
                {
                    Id = a.Id,
                    PublicId = a.PublicId,
                    Nombre = a.Nombre,
                    Codigo = a.Codigo,
                    SucursalId = a.SucursalId,
                    Descripcion = a.Descripcion,
                    EsPrincipal = a.EsPrincipal,
                    Activo = a.Activo,
                    FechaRegistro = a.FechaRegistro,
                    FechaActualizacion = a.FechaActualizacion,
                    Sucursal = new SucursalSlimDto
                    {
                        Id = a.Sucursal.Id,
                        Nombre = a.Sucursal.Nombre
                    },
                    Empresa = new EmpresaSlimDto
                    {
                        Id = a.Sucursal.Empresa.Id,
                        RazonSocial = a.Sucursal.Empresa.RazonSocial
                    }
                })
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<List<ComboDto>> ObtenerCombo(CancellationToken cancellationToken)
            => await _context.Almacenes
                .AsNoTracking()
                .Where(a => a.Activo)
                .Select(a => new ComboDto
                {
                    Id = a.Id,
                    Nombre = a.Nombre
                })
                .ToListAsync(cancellationToken);

        public async Task<int> Crear(Almacen almacen, CancellationToken cancellationToken)
        {
            _context.Almacenes.Add(almacen);
            await _context.SaveChangesAsync(cancellationToken);
            return almacen.Id;
        }

        public async Task Actualizar(Almacen almacen, CancellationToken cancellationToken)
        {
            almacen.FechaActualizacion = DateTime.UtcNow;
            _context.Almacenes.Update(almacen);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Eliminar(int id, CancellationToken cancellationToken)
        {
            var almacen = await _context.Almacenes.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
            if (almacen != null)
            {
                _context.Almacenes.Remove(almacen);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
