using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Api_bd.Services;

namespace Api_bd.Controllers
{
    [ApiController]
    [Route("api/funcionario/dashboard")]
    [Authorize(Roles = "Funcionario,Gestor,Admin")]
    public class DashboardFuncionarioController : ControllerBase
    {
        private readonly IDashboardFuncionarioService _dashboardService;

        public DashboardFuncionarioController(IDashboardFuncionarioService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("cursos-em-andamento")]
        public IActionResult CursosEmAndamento()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var cursos = _dashboardService.GetCursosEmAndamento(userId);
            return Ok(cursos);
        }

        [HttpGet("cursos-disponiveis")]
        public IActionResult CursosDisponiveis()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var cursos = _dashboardService.GetCursosDisponiveis(userId);
            return Ok(cursos);
        }

        [HttpPost("inscrever/{cursoId}")]
        public IActionResult Inscrever(int cursoId)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var result = _dashboardService.Inscrever(userId, cursoId);
            if (!result.Success)
                return BadRequest(new { erro = result.Error });

            return Ok(new { mensagem = "Inscrição realizada com sucesso." });
        }

        [HttpGet("metricas")]
        public IActionResult Metricas()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var metricas = _dashboardService.GetMetricas(userId);
            return Ok(metricas);
        }
    }
}
