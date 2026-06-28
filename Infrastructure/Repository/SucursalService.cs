using Application.Dtos;
using Application.Dtos.Organizacion;
using Application.Interfaces;
using Domain.Catalogo;
using Domain.Organizacion;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class SucursalService : ISucursalService
    {
        private readonly AppDbContext _context;

        public SucursalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Sucursal?> ObtenerPorId(int id, bool tracking, CancellationToken cancellationToken)
            => tracking
                ? await _context.Sucursales.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                : await _context.Sucursales.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<List<Sucursal>> ObtenerTodos(CancellationToken cancellationToken)
            => await _context.Sucursales.ToListAsync(cancellationToken);
        public async Task<List<SucursalDto>> ObtenerTodosOptimizado(CancellationToken cancellationToken)
          => await _context.Sucursales
                .AsNoTracking()
                .Select(a => new SucursalDto
                {
                    Id = a.Id,
                    PublicId = a.PublicId,
                    Nombre = a.Nombre,
                    Codigo = a.Codigo,
                    EmpresaId = a.EmpresaId,
                    PaisId = a.PaisId,
                    Direccion = a.Direccion,
                    Telefono = a.Telefono,
                    EsPrincipal = a.EsPrincipal,
                    Activo = a.Activo,
                    FechaRegistro = a.FechaRegistro,
                    FechaActualizacion = a.FechaActualizacion,
                    Empresa = new EmpresaSlimDto
                    {
                        Id = a.EmpresaId,
                        RazonSocial = a.Empresa.RazonSocial
                    }
                })
                .ToListAsync(cancellationToken);

        public async Task<SucursalDto?> ObtenerPorIdOptimizado(int id, CancellationToken cancellationToken)
             => await _context.Sucursales
                    .AsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(a => new SucursalDto
                    {
                        Id = a.Id,
                        PublicId = a.PublicId,
                        Nombre = a.Nombre,
                        Codigo = a.Codigo,
                        EmpresaId = a.EmpresaId,
                        PaisId = a.PaisId,
                        Direccion = a.Direccion,
                        Telefono = a.Telefono,
                        EsPrincipal = a.EsPrincipal,
                        Activo = a.Activo,
                        FechaRegistro = a.FechaRegistro,
                        FechaActualizacion = a.FechaActualizacion,
                        Empresa = new EmpresaSlimDto
                        {
                            Id = a.Empresa.Id,
                            RazonSocial = a.Empresa.RazonSocial
                        }
                    })
                    .FirstOrDefaultAsync(cancellationToken);

        public async Task<List<ComboDto>> ObtenerCombo(CancellationToken cancellationToken)
            => await _context.Sucursales
                .AsNoTracking()
                .Where(s => s.Empresa.Activo)
                .Select(s => new ComboDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre
                })
                .ToListAsync(cancellationToken);

        public async Task<List<ComboDto>> ObtenerComboByIdEmpresa(int IdEmpresa, 
            CancellationToken cancellationToken)
         => await _context.Sucursales
             .AsNoTracking()
             .Where(s => s.Activo 
                && s.EmpresaId == IdEmpresa)
             .Select(s => new ComboDto
             {
                 Id = s.Id,
                 Nombre = s.Nombre
             })
             .ToListAsync(cancellationToken);

        public async Task<int> Crear(Sucursal sucursal, CancellationToken cancellationToken)
        {
            _context.Sucursales.Add(sucursal);
            await _context.SaveChangesAsync(cancellationToken);
            return sucursal.Id;
        }

        public async Task Actualizar(Sucursal sucursal, CancellationToken cancellationToken)
        {
            sucursal.FechaActualizacion = DateTime.UtcNow;
            _context.Sucursales.Update(sucursal);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Eliminar(int id, CancellationToken cancellationToken)
        {
            var sucursal = await _context.Sucursales.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
            if (sucursal != null)
            {
                _context.Sucursales.Remove(sucursal);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> TieneDependencias(Sucursal sucursal, CancellationToken token)
        {
            var existeAlmacen = await _context.Almacenes
                .AsNoTracking()
                .AnyAsync(p => p.SucursalId == sucursal.Id, token);

            return existeAlmacen;
        }
    }
}
