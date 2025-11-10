using HojeNoRU_API.Models;

namespace HojeNoRU_API.Repositories.Interfaces {
    public interface IRURepository {
        Task<RU?> GetRUByNameAsync(string nome);

        void AddRU(RU ru);
    }
}
