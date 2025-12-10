public class Aula
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Conteudo { get; set; }
    public string? ChaveVideo { get; set; }
    public int Ordem { get; set; }
    public int ModuloId { get; set; }
    public Modulo? Modulo { get; set; }
}