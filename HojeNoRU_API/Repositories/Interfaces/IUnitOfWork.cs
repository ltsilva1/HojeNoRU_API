namespace HojeNoRU_API.Repositories.Interfaces {
    public interface IUnitOfWork : IDisposable {
        IRURepository RUs { get; }
        IRefeicaoRepository Refeicoes { get; }

        Task<int> SaveChanges();
    }
}
