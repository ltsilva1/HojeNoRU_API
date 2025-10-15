using HojeNoRU_API.Context;
using HojeNoRU_API.Models;
using HojeNoRU_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

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

        [HttpGet]
        public ActionResult<IEnumerable<Refeicao>> Get() {
            var refeicoes = _context.Refeicoes.AsNoTracking().Take(10).ToList();
            Console.WriteLine($"Total: {_context.Refeicoes.Count()}");
            return Ok(refeicoes);
        }

        [HttpGet("atualizar")]
        public async Task<IActionResult> Atualizar() {
            await _scraper.AtualizarBancoAsync();
            return Ok("Banco atualizado com sucesso!");
        }
    }
}
