using HojeNoRU_API.Context;
using HojeNoRU_API.Models;
using HojeNoRU_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HojeNoRU_API.Repositories.Interfaces;
using HojeNoRU_API.DTOs.Extensions;
using HojeNoRU_API.DTOs;

namespace HojeNoRU_API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RefeicoesController : ControllerBase {
        private readonly IUnitOfWork _uow;
        private readonly HtmlScraperService _scraper;

        public RefeicoesController(IUnitOfWork uow, HtmlScraperService scraper) {
            _scraper = scraper;
            _uow = uow;
        }

        [HttpGet("atualizar")] // PROTEGER
        public async Task<IActionResult> Atualizar() {
            await _scraper.AtualizarBancoAsync();
            return Ok("Banco atualizado com sucesso!");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RefeicaoDTO>>> GetAll() {
            var refeicoes = await _uow.Refeicoes.GetRefeicoesAsync();
            
            if (!refeicoes.Any())
                return NotFound("Nenhuma refeição encontrada.");

            var refeicoesDto = refeicoes.AsDTO();

            return Ok(refeicoesDto);
        }

        [HttpGet("dia/{diaSemana}")]
        public async Task<ActionResult<IEnumerable<RefeicaoDTO>>> GetPorDia(string diaSemana) {
            var refeicoes = await _uow.Refeicoes.GetRefeicoesAsync(diaSemana: diaSemana);

            if (!refeicoes.Any())
                return NotFound($"Nenhuma refeição encontrada para o dia '{diaSemana}'.");

            var refeicoesDto = refeicoes.AsDTO();

            return Ok(refeicoesDto);
        }

        [HttpGet("dia/{diaSemana}/tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<RefeicaoDTO>>> GetPorDiaETipo(string diaSemana, string tipo) {
            TipoRefeicao tipoEnum;

            if (!Enum.TryParse(tipo, true, out tipoEnum))
                return BadRequest("Tipo inválido. Use 'Almoco' ou 'Jantar'.");

            var refeicoes = await _uow.Refeicoes.GetRefeicoesAsync(diaSemana: diaSemana, tipo: tipoEnum);

            if (!refeicoes.Any())
                return NotFound($"Nenhuma refeição encontrada para {tipoEnum} em '{diaSemana}'.");

            var refeicoesDto = refeicoes.AsDTO();

            return Ok(refeicoesDto);
        }

        [HttpGet("ru/{ruId}")]
        public async Task<ActionResult<IEnumerable<RefeicaoDTO>>> GetPorRU(int ruId) {
            var refeicoes = await _uow.Refeicoes.GetRefeicoesAsync(ruId: ruId);

            if (!refeicoes.Any())
                return NotFound($"Nenhuma refeição encontrada para o RU {ruId}.");

            var refeicoesDto = refeicoes.AsDTO();

            return Ok(refeicoesDto);
        }

        [HttpGet("ru/{ruId}/dia/{diaSemana}")]
        public async Task<ActionResult<IEnumerable<RefeicaoDTO>>> GetPorRUEdia(int ruId, string diaSemana) {
            var refeicoes = await _uow.Refeicoes.GetRefeicoesAsync(ruId: ruId, diaSemana: diaSemana);

            if (!refeicoes.Any())
                return NotFound($"Nenhuma refeição encontrada para o RU {ruId} em '{diaSemana}'.");

            var refeicoesDto = refeicoes.AsDTO();

            return Ok(refeicoesDto);
        }

        [HttpGet("ru/{ruId}/dia/{diaSemana}/tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<RefeicaoDTO>>> GetPorRUEdiaETipo(int ruId, string diaSemana, string tipo) {
            TipoRefeicao tipoEnum;

            if (!Enum.TryParse(tipo, true, out tipoEnum))
                return BadRequest("Tipo inválido. Use 'Almoco' ou 'Jantar'.");

            var refeicoes = await _uow.Refeicoes.GetRefeicoesAsync(ruId: ruId, diaSemana: diaSemana, tipo: tipoEnum);

            if (!refeicoes.Any())
                return NotFound($"Nenhuma refeição encontrada para {tipoEnum} em '{diaSemana}' no RU {ruId}.");

            var refeicoesDto = refeicoes.AsDTO();

            return Ok(refeicoesDto);
        }



    }
}
