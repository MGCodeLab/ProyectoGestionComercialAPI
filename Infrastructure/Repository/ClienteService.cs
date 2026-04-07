using Domain.Catalogo;
using Domain.Comercial;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repository
{
    public class ClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> ObtenerTodos(CancellationToken token)
            => await _context.Clientes.ToListAsync(token);

        public async Task<Cliente?> ObtenerPorId(Guid id, bool isAsTracking, CancellationToken token)
            => (isAsTracking) ?
                await _context.Clientes.FirstOrDefaultAsync(x => x.PublicId == id, token) :
                await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.PublicId == id, token);

        public async Task Crear(Cliente cliente, CancellationToken token)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync(token);
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
