using Microsoft.AspNetCore.Mvc;
using Api_bd.Services;

namespace Api_bd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }


        [HttpGet]
        public ActionResult<List<Usuario>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Usuario> GetById(int id)
        {
            var usuario = _service.GetById(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        [HttpPost]
        public IActionResult Add([FromBody] Usuario usuario)
        {
            var result = _service.Create(usuario);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Usuario);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Usuario usuario)
        {
            var result = _service.Update(id, usuario);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Usuario);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ok = _service.Delete(id);
            if (!ok) return NotFound("Usuário não encontrado.");

            return Ok("Deletado!");
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario login)
        {
            var usuario = _service.Login(login.Email!, login.Senha!);

            if (usuario == null)
                return Unauthorized("Email ou senha incorretos.");

            return Ok("Logado com sucesso!");
        }

    }
}
