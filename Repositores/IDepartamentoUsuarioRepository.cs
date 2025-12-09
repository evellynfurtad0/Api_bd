using System.Collections.Generic;

namespace Api_bd.Repositories
{
    public interface IDepartamentoUsuarioRepository
    {
        List<Usuario> GetUsuariosByDepartamento(int departamentoId);
        bool AddUsuarioToDepartamento(int usuarioId, int departamentoId);
        bool Exists(int usuarioId, int departamentoId);
        List<Departamento> GetDepartamentosDoUsuario(int usuarioId);
    }
}
