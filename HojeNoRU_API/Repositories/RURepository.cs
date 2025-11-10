using HojeNoRU_API.Context;
using HojeNoRU_API.Models;
using HojeNoRU_API.Repositories.Interfaces;
using System.Data.Entity;

namespace HojeNoRU_API.Repositories {
    public class RURepository : IRURepository {
        private readonly AppDbContext _context;

        public RURepository(AppDbContext context) {
            _context = context;
        }

        public void AddRU(RU ru) {
            _context.RUs.Add(ru);
        }

        public async Task<RU?> GetRUByNameAsync(string nome) {
            return await _context.RUs.FirstOrDefaultAsync(r => r.Nome == nome);
        }
    }
}
