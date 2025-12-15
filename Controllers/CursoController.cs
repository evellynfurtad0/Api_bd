using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class CursoController : ControllerBase
{
    private readonly ICursoService _service;

    public CursoController(ICursoService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin,Gestor")]
    [HttpPost("criar-curso")]
    public IActionResult CriarCurso([FromBody] CreateCursoDto dto)
    {
        // pega id do usuário do token
        var idUsuario = int.Parse(User.FindFirst("id")!.Value);
        dto.Usuarios_SistemaId = idUsuario;

        // valida a ordem nos modulos
        if (dto.Modulos.GroupBy(x => x.Ordem).Any(g => g.Count() > 1))
            return BadRequest("Existem módulos com ordem duplicada.");

        // valida a ordem nas aulas
        foreach (var m in dto.Modulos)
        {
            if (m.Aulas.GroupBy(x => x.Ordem).Any(g => g.Count() > 1))
                return BadRequest($"O módulo '{m.Titulo}' possui aulas com ordem duplicada.");
        }
        //chama o service e cria o curso 
        var result = _service.CriarCursoCompleto(dto);

        if (!result.Success)
            return BadRequest(result.Error);

        return Ok(new
        {
            mensagem = "Curso criado com sucesso!",
            cursoId = result.Curso!.Id
        });
    }

    [Authorize(Roles = "Admin,Gestor")]
    [HttpPost("upload-video")]
    public IActionResult UploadVideo(IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Nenhum arquivo enviado.");

        // chave fake,  dps usar a chave do S3
        string chaveGerada = "videos/" + Guid.NewGuid().ToString() + "_" + arquivo.FileName;

        return Ok(new
        {
            mensagem = "Upload recebido (simulado, sem integração S3).",
            chaveVideo = chaveGerada
        });
    }
}
