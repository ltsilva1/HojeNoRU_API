namespace HojeNoRU_API.DTOs.Extensions {
    public static class RefeicaoExtensions {
        public static RefeicaoDTO ToRefeicaoDTO(this Models.Refeicao refeicao) {
            return new RefeicaoDTO {
                Tipo = refeicao.Tipo.ToString(),
                DiaSemana = refeicao.DiaSemana,
                Data = refeicao.Data.ToString("yyyy-MM-dd"),
                RuNome = refeicao.RU?.Nome ?? string.Empty,
                Itens = refeicao.Itens?.Select(i => i.Descricao) ?? Enumerable.Empty<string>()
            };
        }

        public static IEnumerable<RefeicaoDTO> AsDTO(this IEnumerable<Models.Refeicao> refeicoes) {
            return refeicoes.Select(r => r.ToRefeicaoDTO());
        }
    }
}
