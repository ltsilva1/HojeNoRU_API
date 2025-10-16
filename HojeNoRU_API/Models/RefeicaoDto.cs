namespace HojeNoRU_API.Models {
    public class RefeicaoDto { // Pra mostrar os dados na request pública formatadinho
        public string Tipo { get; set; }
        public string DiaSemana { get; set; }
        public string Data { get; set; }
        public string RuNome { get; set; }
        public IEnumerable<string> Itens { get; set; }
    }
}
