public class CreateCursoDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public int Usuarios_SistemaId { get; set; }

    public List<CreateModuloDto> Modulos { get; set; } = new();
}
