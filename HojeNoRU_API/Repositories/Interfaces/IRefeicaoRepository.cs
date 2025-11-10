using HojeNoRU_API.Models;

namespace HojeNoRU_API.Repositories.Interfaces {
    public interface IRefeicaoRepository {
        Task<IEnumerable<Refeicao?>> GetRefeicoesAsync( // subsitui todos os GET
            int? ruId = null,
            string? diaSemana = null,
            TipoRefeicao? tipo = null
        );

        Task<List<Refeicao>> GetAntigasPorRUAsync(int ruID);
        void RemoveRange(IEnumerable<Refeicao> refeicoes);
        void Add(Refeicao refeicao);
    }
}
