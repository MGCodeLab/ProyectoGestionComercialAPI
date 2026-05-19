using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;
public class UnidadMedidaService : IUnidadMedidaService { private readonly AppDbContext _context; public UnidadMedidaService(AppDbContext context) => _context = context; public async Task<List<UnidadMedida>> ObtenerTodos(CancellationToken token) => await _context.UnidadesMedida.AsNoTracking().ToListAsync(token); public async Task<UnidadMedida?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token) => await _context.UnidadesMedida.FirstOrDefaultAsync(u => u.Id == id, token); public async Task<int> Crear(UnidadMedida entity, CancellationToken token) { _context.UnidadesMedida.Add(entity); await _context.SaveChangesAsync(token); return entity.Id; } public async Task Actualizar(CancellationToken token) => await _context.SaveChangesAsync(token); public async Task Eliminar(UnidadMedida entity, CancellationToken token) { _context.UnidadesMedida.Remove(entity); await _context.SaveChangesAsync(token); } }
