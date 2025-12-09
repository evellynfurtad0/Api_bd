public class CreateModuloDto
{
    public string Titulo { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public List<CreateAulaDto> Aulas { get; set; } = new();
}
