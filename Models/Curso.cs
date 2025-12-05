public class Curso
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public int Usuarios_SistemaId { get; set; }

    public Usuario? Usuario { get; set; }
}
