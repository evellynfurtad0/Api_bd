public class Modulo
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public int CursoId { get; set; }
    public Curso? Curso { get; set; }
}
