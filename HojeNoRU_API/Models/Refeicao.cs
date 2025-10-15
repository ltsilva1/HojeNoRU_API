namespace HojeNoRU_API.Models {

    public enum TipoRefeicao {
        Almoco,
        Jantar
    }

    public class Refeicao {
        public int Id { get; set; }
        public TipoRefeicao Tipo { get; set; }

        public string DiaSemana { get; set; } = string.Empty; // Dia da semana e data 
        public DateOnly Data { get; set; }

        public int RUId { get; set; } // FK
        public RU? RU { get; set; }

        public ICollection<ItemCardapio> Itens { get; set; } = new List<ItemCardapio>();
    }
}
