using Api_bd.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Api_bd.Repositories
{
    public class DepartamentoUsuarioRepository : IDepartamentoUsuarioRepository
    {
        private readonly string _connectionString;

        public DepartamentoUsuarioRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }

        public bool Exists(int usuarioId, int departamentoId)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT COUNT(*) FROM DepartamentoUsuario 
                           WHERE Usuarios_SistemaId = @U AND DepartamentoId = @D";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@U", usuarioId);
            cmd.Parameters.AddWithValue("@D", departamentoId);

            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public bool AddUsuarioToDepartamento(int usuarioId, int departamentoId)
        {
            if (Exists(usuarioId, departamentoId))
                return false;

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"INSERT INTO DepartamentoUsuario 
                           (Usuarios_SistemaId, DepartamentoId)
                           VALUES (@U, @D)";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@U", usuarioId);
            cmd.Parameters.AddWithValue("@D", departamentoId);
            cmd.ExecuteNonQuery();

            return true;
        }

        public List<Usuario> GetUsuariosByDepartamento(int departamentoId)
        {
            var list = new List<Usuario>();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                SELECT u.Id, u.Nome, u.Email, u.Senha, u.CPF, u.DataNascimento, u.GestorId
                FROM DepartamentoUsuario du
                JOIN Usuarios_Sistema u ON u.Id = du.Usuarios_SistemaId
                WHERE du.DepartamentoId = @D";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@D", departamentoId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Email = reader.GetString(2),
                    Senha = reader.GetString(3),
                    CPF = reader.IsDBNull(4) ? null : reader.GetString(4),
                    DataNascimento = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    GestorId = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                });
            }

            return list;
        }

        public List<Departamento> GetDepartamentosDoUsuario(int usuarioId)
        {
            var list = new List<Departamento>();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                SELECT d.Id, d.Nome, d.Descricao, d.GestorId
                FROM DepartamentoUsuario du
                JOIN Departamento d ON d.Id = du.DepartamentoId
                WHERE du.Usuarios_SistemaId = @U";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@U", usuarioId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Departamento
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Descricao = reader.IsDBNull(2) ? null : reader.GetString(2),
                    GestorId = reader.IsDBNull(3) ? null : reader.GetInt32(3)
                });
            }

            return list;
        }
    }
}
