using Application.Interfaces;
using Domain.Comercial;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> ObtenerTodos(CancellationToken token)
            => await _context.Clientes.ToListAsync(token);

        public async Task<Cliente?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token)
            => (isAsTracking) ?
                await _context.Clientes.FirstOrDefaultAsync(x => x.Id == id, token) :
                await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);

        public async Task<int> Crear(Cliente cliente, CancellationToken token)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync(token);
            return cliente.Id;
        }

        public async Task Actualizar(CancellationToken token)
            => await _context.SaveChangesAsync(token);

        public async Task Eliminar(Cliente cliente, CancellationToken token)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync(token);
        }
    }
}
