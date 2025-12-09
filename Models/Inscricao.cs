public class Inscricao
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public int Usuarios_SistemaId { get; set; }
    public Curso? Curso { get; set; }
}
