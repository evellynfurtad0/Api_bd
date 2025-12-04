using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Api_bd.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }

        public List<Usuario> GetAll()
        {
            List<Usuario> usuarios = new();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = "SELECT Id, Nome, Email, Senha, Role FROM Usuarios_Sistema";

            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var usuario = new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Email = reader.GetString(2),
                    Senha = reader.GetString(3)
                };

                usuario.Role = usuario.Role = (RoleType)reader.GetInt32(4);

                usuarios.Add(usuario);
            }

            return usuarios;
        }

        public Usuario? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = "SELECT Id, Nome, Email, Senha, Role FROM Usuarios_Sistema WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            var usuario = new Usuario
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Email = reader.GetString(2),
                Senha = reader.GetString(3)
            };

            usuario.Role = usuario.Role = (RoleType)reader.GetInt32(4);

            return usuario;
        }

        public Usuario Create(Usuario usuario)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"INSERT INTO Usuarios_Sistema (Nome, Email, Senha, Role)
                           VALUES (@Nome, @Email, @Senha, @Role);
                           SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
            cmd.Parameters.AddWithValue("@Email", usuario.Email);
            cmd.Parameters.AddWithValue("@Senha", usuario.Senha);
            cmd.Parameters.AddWithValue("@Role", (int)usuario.Role);

            var result = cmd.ExecuteScalar();
            usuario.Id = Convert.ToInt32(result);

            return usuario;
        }

        public Usuario? Update(int id, Usuario usuario)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"UPDATE Usuarios_Sistema 
               SET Nome = @Nome, Email = @Email, Senha = @Senha, Role = @Role
               WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
            cmd.Parameters.AddWithValue("@Email", usuario.Email);
            cmd.Parameters.AddWithValue("@Senha", usuario.Senha);
            cmd.Parameters.AddWithValue("@Role", (int)usuario.Role);
            cmd.Parameters.AddWithValue("@Id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows == 0) return null;

            usuario.Id = id;
            return usuario;
        }

        public Usuario? Login(string email, string senha)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Nome, Email, Senha, Role
                        FROM Usuarios_Sistema
                        WHERE Email = @Email AND Senha = @Senha";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Senha", senha);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            var usuario = new Usuario
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Email = reader.GetString(2),
                Senha = reader.GetString(3)
            };

            usuario.Role = (RoleType)reader.GetInt32(4);

            return usuario;
        }

        public bool Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = "DELETE FROM Usuarios_Sistema WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ExistsByEmail(string email)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = "SELECT COUNT(*) FROM Usuarios_Sistema WHERE Email = @Email";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Email", email);

            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
