namespace HojeNoRU_API.Models {
    public class ItemCardapio {
        public int Id { get; set; }

        public string Descricao { get; set; } = string.Empty; // O que o prato é. Ex: "Frango grelhado", "Lasanha de legumes"

        // FK
        public int RefeicaoId { get; set; }
        public Refeicao? Refeicao { get; set; }
    }
}
