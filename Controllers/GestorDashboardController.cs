using Api_bd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_bd.Controllers
{
    [ApiController]
    [Route("api/gestor/dashboard")]
    [Authorize(Roles = "Gestor")]
    public class DashboardGestorController : ControllerBase
    {
        private readonly IGestorDashboardService _service;

        public DashboardGestorController(IGestorDashboardService service)
        {
            _service = service;
        }

        private int GetGestorId()
        {
            return int.Parse(User.FindFirst("id")!.Value);
        }

        [HttpGet("departamento")]
        public IActionResult GetDepartamentos()
        {
            var gestorId = GetGestorId();
            return Ok(_service.GetDepartamentosGerenciados(gestorId));
        }

        [HttpPost("departamento/{departamentoId}/adicionar-usuario-no-departamento/{usuarioId}")]
        public IActionResult AdicionarUsuarioNoDepartamento(int departamentoId, int usuarioId)
        {
            var result = _service.AdicionarUsuarioAoDepartamento(departamentoId, usuarioId);
            if (!result.Success) return BadRequest(result.Error);
            return Ok("Usuário adicionado ao departamento.");
        }

        [HttpPost("atribuir-curso-departamento")]
        public IActionResult AtribuirCurso([FromQuery] int cursoId, [FromQuery] int departamentoId)
        {
            var gestorId = GetGestorId();
            var result = _service.AtribuirCursoAoDepartamento(gestorId, cursoId, departamentoId);

            if (!result.Success) return BadRequest(result.Error);

            return Ok(new
            {
                Mensagem = "Curso atribuído ao departamento.",
                Inscritos = result.Inscritos
            });
        }

        [HttpGet("departamento/{departamentoId}/cursos")]
        public IActionResult GetCursosDoDepartamento(int departamentoId)
        {
            return Ok(_service.GetCursosDoDepartamento(departamentoId));
        }

        [HttpGet("departamento/{departamentoId}/progresso")]
        public IActionResult GetProgressoDoDepartamento(int departamentoId)
        {
            return Ok(_service.GetProgressoPorUsuarioDoDepartamento(departamentoId));
        }

        [HttpGet("metricas")]
        public IActionResult GetMetricas()
        {
            var gestorId = GetGestorId();
            return Ok(_service.GetMetricasPorDepartamentosGerenciados(gestorId));
        }
    }
}