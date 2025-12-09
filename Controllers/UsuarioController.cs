using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api_bd.Services;
using Microsoft.Extensions.Configuration;

namespace Api_bd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly IConfiguration _configuration;

        public UsuarioController(IUsuarioService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var usuarios = _service.GetAll()
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email
                });

            return Ok(usuarios);
        }

        [Authorize(Roles = "Admin,Gestor")]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var u = _service.GetById(id);
            if (u == null) return NotFound();

            return Ok(new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Add([FromBody] Usuario usuario)
        {
            var result = _service.Create(usuario);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Usuario);
        }

        [Authorize(Roles = "Admin,Gestor")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Usuario usuario)
        {
            var result = _service.Update(id, usuario);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Usuario);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ok = _service.Delete(id);
            if (!ok) return NotFound("Usuário não encontrado.");

            return Ok("Deletado!");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            //verifica se a senha e email estão no banco
            var usuario = _service.Login(login.Email!, login.Senha!);

            if (usuario == null)
                return Unauthorized("Email ou senha incorretos.");

            // pega a chave do appsettings
            var chave = _configuration["Jwt:SigningKey"];
            if (string.IsNullOrWhiteSpace(chave))
                return StatusCode(500, "Chave JWT não configurada.");

            // gera o token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(chave);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //define os dados que ficarão dentro do token JWT
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
                }),

                //tempo de expiração do token (2hrs)
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //retorna o token
            return Ok(new
            {
                mensagem = "Logado com sucesso!",
                token = tokenString,
                usuario = usuario.Nome
            });
        }
    }
}
