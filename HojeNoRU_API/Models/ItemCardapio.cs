namespace HojeNoRU_API.Models {
    public class ItemCardapio {
        public int Id { get; set; }

        // public string Categoria { get; set; } = string.Empty; // "Prato Principal", "Vegana", "Salada", etc.
        public string Descricao { get; set; } = string.Empty; // "Frango grelhado", "Lasanha de legumes"...

        // FK
        public int RefeicaoId { get; set; }
        public Refeicao? Refeicao { get; set; }
    }
}
