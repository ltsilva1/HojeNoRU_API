using HojeNoRU_API.Context;
using HojeNoRU_API.Repositories.Interfaces;

namespace HojeNoRU_API.Repositories {
    public class UnitOfWork : IUnitOfWork {
        private readonly AppDbContext _context;

        public IRURepository RUs { get; private set; }
        public IRefeicaoRepository Refeicoes { get; private set; }

        public UnitOfWork(AppDbContext context) {
            _context = context;
            RUs = new RURepository(_context);
            Refeicoes = new RefeicaoRepository(_context);
        }

        public void Dispose() {
            _context.Dispose();
        }

        public async Task<int> SaveChanges() {
            return await _context.SaveChangesAsync();
        }
    }
}
