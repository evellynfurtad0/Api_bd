public class ProgressoCurso
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public int CursoId { get; set; }
    public int ModuloId { get; set; }
    public int AulaId { get; set; }

    public string Status { get; set; } = string.Empty;

    public Usuario? Usuario { get; set; }
    public Curso? Curso { get; set; }
    public Modulo? Modulo { get; set; }
    public Aula? Aula { get; set; }
}
