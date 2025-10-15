namespace HojeNoRU_API.Models {
    public class RU {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty; // o nome do RU

        public ICollection<Refeicao> Refeicoes { get; set; } = new List<Refeicao>();
    }
}
