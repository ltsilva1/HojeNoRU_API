using HojeNoRU_API.Context;
using HojeNoRU_API.Models;
using HojeNoRU_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HojeNoRU_API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RefeicoesController : ControllerBase {
        private readonly AppDbContext _context;
        private readonly HtmlScraperService _scraper;

        public RefeicoesController(AppDbContext context, HtmlScraperService scraper) {
            _context = context;
            _scraper = scraper;
        }

        [HttpGet("atualizar")] // PROTEGER
        public async Task<IActionResult> Atualizar() {
            await _scraper.AtualizarBancoAsync();
            return Ok("Banco atualizado com sucesso!");
        }

        [HttpGet]
        public ActionResult<IEnumerable<Refeicao>> Get() {
            var refeicoes = _context.Refeicoes
                           .AsNoTracking()
                           .Include(r => r.RU)
                           .Include(r => r.Itens)
                           .OrderBy(r => r.Data)
                           .Select(r => new RefeicaoDto {
                               Tipo = r.Tipo.ToString(),
                               DiaSemana = r.DiaSemana,
                               Data = r.Data.ToString("yyyy-MM-dd"),
                               RuNome = r.RU.Nome,
                               Itens = r.Itens.Select(i => i.Descricao)
                           })
                           .ToList();
            Console.WriteLine("Total: " + _context.Refeicoes.Count());
            return Ok(refeicoes);
        }

        [HttpGet("dia/{diaSemana}")]
        public ActionResult<IEnumerable<Refeicao>> GetPorDia(string diaSemana) {
            var refeicoes = _context.Refeicoes
                .Include(r => r.RU)
                .Include(r => r.Itens)
                .Where(r => r.DiaSemana.ToLower() == diaSemana.ToLower())
                .OrderBy(r => r.RU.Nome)
                .ThenBy(r => r.Tipo)
                .Select(r => new RefeicaoDto {
                    Tipo = r.Tipo.ToString(),
                    DiaSemana = r.DiaSemana,
                    Data = r.Data.ToString("yyyy-MM-dd"),
                    RuNome = r.RU.Nome,
                    Itens = r.Itens.Select(i => i.Descricao)
                })
                .ToList();

            if (!refeicoes.Any())
                return NotFound($"Nenhuma refeição encontrada para o dia '{diaSemana}'.");

            return Ok(refeicoes);
        }

        [HttpGet("dia/{diaSemana}/tipo/{tipo}")]
        public ActionResult<IEnumerable<Refeicao>> GetPorDiaETipo(string diaSemana, string tipo) {
            TipoRefeicao tipoEnum;

            if (!Enum.TryParse(tipo, true, out tipoEnum))
                return BadRequest("Tipo inválido. Use 'Almoco' ou 'Jantar'.");

            var refeicoes = _context.Refeicoes
                .Include(r => r.RU)
                .Include(r => r.Itens)
                .Where(r => r.DiaSemana.ToLower() == diaSemana.ToLower() && r.Tipo == tipoEnum)
                .OrderBy(r => r.RU.Nome)
                .Select(r => new RefeicaoDto {
                    Tipo = r.Tipo.ToString(),
                    DiaSemana = r.DiaSemana,
                    Data = r.Data.ToString("yyyy-MM-dd"),
                    RuNome = r.RU.Nome,
                    Itens = r.Itens.Select(i => i.Descricao)
                })
                .ToList();

            if (!refeicoes.Any())
                return NotFound($"Nenhuma refeição encontrada para {tipoEnum} em '{diaSemana}'.");

            return Ok(refeicoes);
        }

        [HttpGet("ru/{ruId}")]
        public ActionResult<IEnumerable<Refeicao>> GetPorRU(int ruId) {
            var refeicoes = _context.Refeicoes
                .Include(r => r.RU)
                .Include(r => r.Itens)
                .Where(r => r.RUId == ruId)
                .OrderBy(r => r.DiaSemana) // NOTA: Ele vai retornar por ORDEM ALFABÉTICA, ou seja, quarta vai vir sempre antes da segunda (vale corrigir)
                .ThenBy(r => r.Tipo)
                .Select(r => new RefeicaoDto {
                    Tipo = r.Tipo.ToString(),
                    DiaSemana = r.DiaSemana,
                    Data = r.Data.ToString("yyyy-MM-dd"),
                    RuNome = r.RU.Nome,
                    Itens = r.Itens.Select(i => i.Descricao)
                })
                .ToList();

            if (!refeicoes.Any())
                return NotFound($"Nenhuma refeição encontrada para o RU {ruId}.");

            return Ok(refeicoes);
        }

        [HttpGet("ru/{ruId}/dia/{diaSemana}")]
        public ActionResult<IEnumerable<Refeicao>> GetPorRUEdia(int ruId, string diaSemana) {
            var refeicoes = _context.Refeicoes
                .Include(r => r.RU)
                .Include(r => r.Itens)
                .Where(r => r.RUId == ruId && r.DiaSemana.ToLower() == diaSemana.ToLower())
                .OrderBy(r => r.Tipo)
                .Select(r => new RefeicaoDto {
                    Tipo = r.Tipo.ToString(),
                    DiaSemana = r.DiaSemana,
                    Data = r.Data.ToString("yyyy-MM-dd"),
                    RuNome = r.RU.Nome,
                    Itens = r.Itens.Select(i => i.Descricao)
                })
                .ToList();

            if (!refeicoes.Any())
                return NotFound($"Nenhuma refeição encontrada para o RU {ruId} em '{diaSemana}'.");

            return Ok(refeicoes);
        }

        [HttpGet("ru/{ruId}/dia/{diaSemana}/tipo/{tipo}")]
        public ActionResult<IEnumerable<Refeicao>> GetPorRUEdiaETipo(int ruId, string diaSemana, string tipo) {
            TipoRefeicao tipoEnum;

            if (!Enum.TryParse(tipo, true, out tipoEnum))
                return BadRequest("Tipo inválido. Use 'Almoco' ou 'Jantar'.");

            var refeicoes = _context.Refeicoes
                .Include(r => r.RU)
                .Include(r => r.Itens)
                .Where(r => r.RUId == ruId && r.DiaSemana.ToLower() == diaSemana.ToLower() && r.Tipo == tipoEnum)
                .OrderBy(r => r.RU.Nome)
                .Select(r => new RefeicaoDto {
                    Tipo = r.Tipo.ToString(),
                    DiaSemana = r.DiaSemana,
                    Data = r.Data.ToString("yyyy-MM-dd"),
                    RuNome = r.RU.Nome,
                    Itens = r.Itens.Select(i => i.Descricao)
                })
                .ToList();

            if (!refeicoes.Any())
                return NotFound($"Nenhuma refeição encontrada para {tipoEnum} em '{diaSemana}' no RU {ruId}.");

            return Ok(refeicoes);
        }



    }
}
