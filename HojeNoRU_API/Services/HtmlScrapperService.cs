using HojeNoRU_API.Context;
using HojeNoRU_API.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace HojeNoRU_API.Services {
    public class HtmlScraperService {
        private readonly AppDbContext _context;
        private readonly HttpClient _http;

        public HtmlScraperService(AppDbContext context, HttpClient http) {
            _context = context;
            _http = http;
        }

        public async Task AtualizarBancoAsync() {
            var url = "https://www.ufrgs.br/prae/cardapio-ru/";
            var html = await _http.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var blocosRU = doc.DocumentNode.SelectNodes("//div[contains(@class,'elementor-widget-wrap')]");
            if (blocosRU == null) return;

            foreach (var bloco in blocosRU) {
                var nomeRU = bloco.SelectSingleNode(".//h3")?.InnerText.Trim();
                if (string.IsNullOrEmpty(nomeRU)) continue;

                // pra garantir que o RU existe no DB
                var ru = await _context.RUs.FirstOrDefaultAsync(r => r.Nome == nomeRU);
                if (ru == null) {
                    ru = new RU { Nome = nomeRU };
                    _context.RUs.Add(ru);
                    await _context.SaveChangesAsync();
                }

                var tabelas = bloco.SelectNodes(".//div[contains(@class,'elementor-toggle-item')]//table");
                if (tabelas == null) continue;

                foreach (var tabela in tabelas) {
                    var tituloNode = tabela.ParentNode.ParentNode.SelectSingleNode(".//a[@class='elementor-toggle-title']");
                    string textoTipo = tituloNode?.InnerText.Trim() ?? "Desconhecido";
                    var tipo = textoTipo.ToLower().Contains("almoço") ? TipoRefeicao.Almoco : TipoRefeicao.Jantar;

                    var linhas = tabela.SelectNodes(".//tr")?.ToList();
                    if (linhas == null || linhas.Count == 0) continue;

                    var dias = linhas[0].SelectNodes(".//td").Select(d => d.InnerText.Trim()).ToList();

                    // pratos por dia
                    var linhasPratos = linhas.Skip(1).ToList();

                    for (int i = 0; i < dias.Count; i++) {
                        var diaTexto = dias[i];
                        if (string.IsNullOrWhiteSpace(diaTexto)) continue;

                        // Extrair data do texto
                        var data = ExtrairData(diaTexto);

                        var refeicao = new Refeicao {
                            Tipo = tipo,
                            DiaSemana = diaTexto,
                            Data = data,
                            RUId = ru.Id,
                            Itens = new List<ItemCardapio>()
                        };

                        foreach (var linha in linhasPratos) {
                            var colunas = linha.SelectNodes(".//td");
                            if (colunas == null || i >= colunas.Count) continue;

                            var prato = colunas[i].InnerText.Trim();
                            if (!string.IsNullOrWhiteSpace(prato)) {
                                refeicao.Itens.Add(new ItemCardapio { Descricao = prato });
                            }
                        }

                        // Para evitar duplicar registros de mesmo RU, dia e tipo
                        var existente = await _context.Refeicoes
                            .Include(r => r.Itens)
                            .FirstOrDefaultAsync(r =>
                                r.RUId == ru.Id &&
                                r.Data == refeicao.Data &&
                                r.Tipo == refeicao.Tipo);

                        if (existente != null) {
                            await _context.SaveChangesAsync(); // pra garantir que IDs foram gerados
                            _context.ItensCardapio.RemoveRange(existente.Itens);
                            existente.Itens = refeicao.Itens;
                        } else {
                            _context.Refeicoes.Add(refeicao);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private DateOnly ExtrairData(string diaTexto) { // Exemplo de string: Quarta-feira (16/10)
            var match = System.Text.RegularExpressions.Regex.Match(diaTexto, @"\((\d{1,2})/(\d{1,2})\)");
            if (match.Success) {
                int dia = int.Parse(match.Groups[1].Value);
                int mes = int.Parse(match.Groups[2].Value);
                int ano = DateTime.Now.Year;
                return new DateOnly(ano, mes, dia);
            }
            return DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
