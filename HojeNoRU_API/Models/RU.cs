using System.Text.Json.Serialization;

namespace HojeNoRU_API.Models {
    public class RU {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty; // o nome do RU

        [JsonIgnore]  // Para evitar loops e [null] (redundante?)
        public ICollection<Refeicao> Refeicoes { get; set; } = new List<Refeicao>();
    }
}
