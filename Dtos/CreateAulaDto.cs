public class CreateAulaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Conteudo { get; set; }
    public string? ChaveVideo { get; set; }
    public int Ordem { get; set; }
}
