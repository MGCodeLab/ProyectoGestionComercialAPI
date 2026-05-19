using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Domain.Configuracion;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;
public class ParametroSistemaService : IParametroSistemaService { private readonly AppDbContext _context; public ParametroSistemaService(AppDbContext context) => _context = context; public async Task<List<ParametroSistema>> ObtenerTodos(CancellationToken token) => await _context.ParametrosSistema.AsNoTracking().ToListAsync(token); public async Task<ParametroSistema?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token) => await _context.ParametrosSistema.FirstOrDefaultAsync(p => p.Id == id, token); public async Task<int> Crear(ParametroSistema entity, CancellationToken token) { _context.ParametrosSistema.Add(entity); await _context.SaveChangesAsync(token); return entity.Id; } public async Task Actualizar(CancellationToken token) => await _context.SaveChangesAsync(token); public async Task Eliminar(ParametroSistema entity, CancellationToken token) { _context.ParametrosSistema.Remove(entity); await _context.SaveChangesAsync(token); } }
