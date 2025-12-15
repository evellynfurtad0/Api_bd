using Api_bd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_bd.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardAdminController : ControllerBase
    {
        private readonly IAdminDashboardService _service;

        public DashboardAdminController(IAdminDashboardService service)
        {
            _service = service;
        }

        [HttpGet("todos-cursos")]
        public IActionResult GetCursosRecentes()
        {
            return Ok(_service.GetCursosRecentes());
        }

        [HttpGet("departamentos")]
        public IActionResult GetEquipes()
        {
            return Ok(_service.GetTodasEquipes());
        }

        [HttpGet("metricas")]
        public IActionResult GetMetricas()
        {
            return Ok(_service.GetMetricasGerais());
        }

        [HttpPost("promover-gestor/{usuarioId}")]
        public IActionResult PromoverGestor(int usuarioId)
        {
            var result = _service.PromoverParaGestor(usuarioId);

            if (!result.Success)
                return BadRequest(new { erro = result.Error });

            return Ok(new { mensagem = "Usuário promovido a gestor com sucesso!" });
        }

        [HttpPost("departamentos")]
        public IActionResult CriarDepartamento([FromBody] Departamento novo)
        {
            var result = _service.CriarDepartamento(novo);

            if (!result.Success)
                return BadRequest(new { erro = result.Error });

            return Ok(new { mensagem = "Departamento criado com sucesso!", departamento = result.Departamento });
        }

        [HttpGet("departamentos-com-usuarios")]
        public IActionResult GetDepartamentosComUsuarios()
        {
            return Ok(_service.GetDepartamentosComUsuarios());
        }
    }
}