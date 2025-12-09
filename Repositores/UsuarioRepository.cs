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

            //inicia a conexão com o banco
            using var con = new SqlConnection(_connectionString);
            con.Open();

            //consulta no banco
            string sql = @"SELECT Id, Nome, Email, Senha, Perfil, CPF, DataNascimento, GestorId 
                           FROM Usuarios_Sistema";

            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var usuario = new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Email = reader.GetString(2),
                    Senha = reader.GetString(3),

                    Perfil = (PerfilEnum)reader.GetInt32(4),

                    CPF = reader.IsDBNull(5) ? null : reader.GetString(5),
                    DataNascimento = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    GestorId = reader.IsDBNull(7) ? null : reader.GetInt32(7)
                };

                usuarios.Add(usuario);
            }

            return usuarios;
        }

        public Usuario? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Nome, Email, Senha, Perfil, CPF, DataNascimento, GestorId 
                           FROM Usuarios_Sistema 
                           WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            var usuario = new Usuario
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Email = reader.GetString(2),
                Senha = reader.GetString(3),

                Perfil = (PerfilEnum)reader.GetInt32(4),

                CPF = reader.IsDBNull(5) ? null : reader.GetString(5),
                DataNascimento = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                GestorId = reader.IsDBNull(7) ? null : reader.GetInt32(7)
            };

            return usuario;
        }

        public Usuario Create(Usuario usuario)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                INSERT INTO Usuarios_Sistema 
                (Nome, Email, Senha, Perfil, CPF, DataNascimento, GestorId)
                VALUES 
                (@Nome, @Email, @Senha, @Perfil, @CPF, @DataNascimento, @GestorId);
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
            cmd.Parameters.AddWithValue("@Email", usuario.Email);
            cmd.Parameters.AddWithValue("@Senha", usuario.Senha);
            cmd.Parameters.AddWithValue("@Perfil", (int)usuario.Perfil);

            cmd.Parameters.AddWithValue("@CPF", (object?)usuario.CPF ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DataNascimento", (object?)usuario.DataNascimento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GestorId",usuario.GestorId == null || usuario.GestorId == 0 ? DBNull.Value : usuario.GestorId);

            var result = cmd.ExecuteScalar();
            usuario.Id = Convert.ToInt32(result);

            return usuario;
        }

        public Usuario? Update(int id, Usuario usuario)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                UPDATE Usuarios_Sistema 
                SET 
                    Nome = @Nome, 
                    Email = @Email, 
                    Senha = @Senha, 
                    Perfil = @Perfil,
                    CPF = @CPF,
                    DataNascimento = @DataNascimento,
                    GestorId = @GestorId
                WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
            cmd.Parameters.AddWithValue("@Email", usuario.Email);
            cmd.Parameters.AddWithValue("@Senha", usuario.Senha);
            cmd.Parameters.AddWithValue("@Perfil", (int)usuario.Perfil);

            cmd.Parameters.AddWithValue("@CPF", (object?)usuario.CPF ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DataNascimento", (object?)usuario.DataNascimento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GestorId", usuario.GestorId == null || usuario.GestorId == 0 ? DBNull.Value : usuario.GestorId);

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

            string sql = @"
                SELECT Id, Nome, Email, Senha, Perfil, CPF, DataNascimento, GestorId
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
                Senha = reader.GetString(3),

                Perfil = (PerfilEnum)reader.GetInt32(4),

                CPF = reader.IsDBNull(5) ? null : reader.GetString(5),
                DataNascimento = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                GestorId = reader.IsDBNull(7) ? null : reader.GetInt32(7)
            };

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
