namespace Api_bd.Repositories
{
    public interface IUsuarioRepository
    {
        List<Usuario> GetAll();
        Usuario? GetById(int id);
        Usuario Create(Usuario usuario);
        Usuario? Update(int id, Usuario usuario);
        Usuario? Login(string email, string senha);
        bool Delete(int id);
        bool ExistsByEmail(string email);
    }
}
