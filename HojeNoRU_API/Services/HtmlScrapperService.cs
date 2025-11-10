using HojeNoRU_API.Context;
using HojeNoRU_API.Models;
using HojeNoRU_API.Repositories.Interfaces;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace HojeNoRU_API.Services {
    public class HtmlScraperService {
        private readonly IUnitOfWork _uow;
        private readonly HttpClient _http;

        public HtmlScraperService(IUnitOfWork uow, HttpClient http) {
            _uow = uow;
            _http = http;
        }

        public async Task AtualizarBancoAsync() {
            var url = "https://www.ufrgs.br/prae/cardapio-ru/";
            var html = await _http.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var blocosRU = doc.DocumentNode.SelectNodes("//div[contains(@class,'elementor-widget-wrap')]");
            if (blocosRU is null) return;

            foreach (var bloco in blocosRU) {
                var nomeRU = bloco.SelectSingleNode(".//h3")?.InnerText.Trim();
                if (string.IsNullOrEmpty(nomeRU)) continue;

                // pra garantir que o RU existe no DB
                var ru = await _uow.RUs.GetRUByNameAsync(nomeRU);
                if (ru is null) {
                    ru = new RU { Nome = nomeRU };
                    _uow.RUs.AddRU(ru);
                    await _uow.SaveChanges();
                }

                var refeicoesAntigas = await _uow.Refeicoes.GetAntigasPorRUAsync(ru.Id);

                if (refeicoesAntigas.Any()) { // limpa o cardápio antigo antes de adicionar o novo pra evitar duplicatas
                    Console.WriteLine($"Removendo {refeicoesAntigas.Count} refeições antigas de {ru.Nome}...");
                    _uow.Refeicoes.RemoveRange(refeicoesAntigas);
                    // await _uow.SaveChanges(); desnecessário salvar aqui (eu acho)
                }

                var tabelas = bloco.SelectNodes(".//div[contains(@class,'elementor-toggle-item')]//table");
                if (tabelas is null) continue;

                foreach (var tabela in tabelas) {
                    var tituloNode = tabela.ParentNode.ParentNode.SelectSingleNode(".//a[@class='elementor-toggle-title']");
                    string textoTipo = tituloNode?.InnerText.Trim() ?? "Desconhecido";
                    var tipo = textoTipo.ToLower().Contains("almoço") ? TipoRefeicao.Almoco : TipoRefeicao.Jantar;

                    var linhas = tabela.SelectNodes(".//tr")?.ToList();
                    if (linhas is null || linhas.Count == 0) continue;

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
                            if (colunas is null || i >= colunas.Count) continue;

                            var prato = colunas[i].InnerText.Trim();
                            if (!string.IsNullOrWhiteSpace(prato)) {
                                refeicao.Itens.Add(new ItemCardapio { Descricao = prato });
                            }
                        }

                        _uow.Refeicoes.Add(refeicao);

                    }
                }
            }

            await _uow.SaveChanges();
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
