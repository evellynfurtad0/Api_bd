using System.Collections.Generic;

namespace Api_bd.Services
{
    public interface IUsuarioService
    {
        List<Usuario> GetAll();
        Usuario? GetById(int id);
        (bool IsSuccess, string? ErrorMessage, Usuario? Usuario) Create(Usuario usuario);
        (bool IsSuccess, string? ErrorMessage, Usuario? Usuario) Update(int id, Usuario usuario);
        bool Delete(int id);
        Usuario? Login(string email, string senha);
    }
}