namespace HojeNoRU_API.DTOs {
    public class RefeicaoDTO {
        public string Tipo { get; set; }
        public string DiaSemana { get; set; }
        public string Data { get; set; }
        public string RuNome { get; set; }
        public IEnumerable<string> Itens { get; set; }
    }
}
