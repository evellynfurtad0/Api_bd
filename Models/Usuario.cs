using System.ComponentModel.DataAnnotations;

public class Usuario
{
    public int Id { get; set; }

    [Required, MinLength(2)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(3)]
    public string Senha { get; set; } = string.Empty;

    public string? CPF { get; set; } = null;

    public DateTime? DataNascimento { get; set; } = null;

    public int? GestorId { get; set; }

    public PerfilEnum Perfil { get; set; } = PerfilEnum.Funcionario;
}
