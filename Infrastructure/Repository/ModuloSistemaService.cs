using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Domain.Configuracion;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;
public class ModuloSistemaService : IModuloSistemaService 
{ 
    private readonly AppDbContext _context; 

    public ModuloSistemaService(AppDbContext context) => _context = context; 
    
    public async Task<List<ModuloSistema>> ObtenerTodos(CancellationToken token) 
        => await _context.ModulosSistema.AsNoTracking().ToListAsync(token); 
    
    public async Task<ModuloSistema?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token) 
        => await _context.ModulosSistema.FirstOrDefaultAsync(m => m.Id == id, token); 
    
    public async Task<int> Crear(ModuloSistema entity, CancellationToken token) { 
        _context.ModulosSistema.Add(entity); 
        await _context.SaveChangesAsync(token); 
        return entity.Id; 
    } 
    
    public async Task Actualizar(CancellationToken token) 
        => await _context.SaveChangesAsync(token); 
    
    public async Task Eliminar(ModuloSistema entity, CancellationToken token) 
    { 
        _context.ModulosSistema.Remove(entity); 
        await _context.SaveChangesAsync(token); 
    } 
}
