using Api_bd.Repositories;
using System.Collections.Generic;

namespace Api_bd.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public List<Usuario> GetAll()
        {
            return _repository.GetAll();
        }

        public Usuario? GetById(int id)
        {
            return _repository.GetById(id);
        }

        public (bool IsSuccess, string? ErrorMessage, Usuario? Usuario) Create(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nome))
                return (false, "Nome é obrigatório.", null);

            if (string.IsNullOrWhiteSpace(usuario.Email))
                return (false, "Email é obrigatório.", null);

            if (_repository.ExistsByEmail(usuario.Email!))
                return (false, "Email já cadastrado.", null);

            var created = _repository.Create(usuario);
            return (true, null, created);
        }

        public (bool IsSuccess, string? ErrorMessage, Usuario? Usuario) Update(int id, Usuario usuario)
        {
            var existing = _repository.GetById(id);
            if (existing == null)
                return (false, "Usuário não encontrado.", null);

            if (string.IsNullOrWhiteSpace(usuario.Nome))
                return (false, "Nome é obrigatório.", null);

            if (string.IsNullOrWhiteSpace(usuario.Email))
                return (false, "Email é obrigatório.", null);

            if (!string.Equals(existing.Email, usuario.Email, System.StringComparison.OrdinalIgnoreCase)
                && _repository.ExistsByEmail(usuario.Email!))
            {
                return (false, "Email já cadastrado por outro usuário.", null);
            }

            var updated = _repository.Update(id, usuario);
            return (true, null, updated);
        }

        public Usuario? Login(string email, string senha)
        {
            return _repository.Login(email, senha);
        }

        public bool Delete(int id)
        {
            return _repository.Delete(id);
        }
    }
}