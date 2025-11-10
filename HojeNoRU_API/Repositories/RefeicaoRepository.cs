using HojeNoRU_API.Context;
using HojeNoRU_API.Models;
using HojeNoRU_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HojeNoRU_API.Repositories {
    public class RefeicaoRepository : IRefeicaoRepository {
        private readonly AppDbContext _context;

        public RefeicaoRepository(AppDbContext context) {
            _context = context;
        }

        public void Add(Refeicao refeicao) {
            _context.Refeicoes.Add(refeicao);
        }

        public Task<List<Refeicao>> GetAntigasPorRUAsync(int ruID) { // separado pro SCRAPPER
            return _context.Refeicoes
                .Include(r => r.Itens)
                .Where(r => r.RUId == ruID)
                .ToListAsync();
        }

        public async Task<IEnumerable<Refeicao?>> GetRefeicoesAsync(int? ruId = null, string? diaSemana = null, TipoRefeicao? tipo = null) {
            var query = _context.Refeicoes
                                .Include(r => r.RU)
                                .Include(r => r.Itens)
                                .AsQueryable();

            if (ruId.HasValue) {
                query = query.Where(r => r.RUId == ruId.Value);
            }
            if (!string.IsNullOrEmpty(diaSemana)) {
                query = query.Where(r => r.DiaSemana.ToLower() == diaSemana.ToLower());
            }
            if (tipo.HasValue) {
                query = query.Where(r => r.Tipo == tipo.Value);
            }

            query = query.OrderBy(r => r.DiaSemana) // tem que arrumar a ordem alfabética!!!
                         .ThenBy(r => r.Tipo);

            return await query.AsNoTracking().ToListAsync(); // consulta só eh executada aqui
        }

        public void RemoveRange(IEnumerable<Refeicao> refeicoes) {
            var itensParaRemover = refeicoes.SelectMany(r => r.Itens);

            _context.ItensCardapio.RemoveRange(itensParaRemover);
            _context.Refeicoes.RemoveRange(refeicoes);
        }
    }
}
