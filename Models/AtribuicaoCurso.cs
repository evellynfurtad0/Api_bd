public class AtribuicaoCurso
{
    public int Id { get; set; }
    public int GestorId { get; set; }
    public int CursoId { get; set; }
    public int? DepartamentoId { get; set; }
    public Usuario? Gestor { get; set; }
    public Curso? Curso { get; set; }
}
