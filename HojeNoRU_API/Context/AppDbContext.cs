using Microsoft.EntityFrameworkCore;
using HojeNoRU_API.Models;


namespace HojeNoRU_API.Context {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<RU> RUs { get; set; }
        public DbSet<ItemCardapio> ItensCardapio { get; set; }
        public DbSet<Refeicao> Refeicoes { get; set; }
    }
}
