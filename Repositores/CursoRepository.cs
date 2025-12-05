using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Api_bd.Repositories
{
    public class CursoRepository : ICursoRepository
    {
        private readonly string _connectionString;

        public CursoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }

        public List<Curso> GetAll()
        {
            List<Curso> cursos = new();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Titulo, Descricao, Usuarios_SistemaId 
                           FROM Curso";

            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cursos.Add(new Curso
                {
                    Id = reader.GetInt32(0),
                    Titulo = reader.GetString(1),
                    Descricao = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Usuarios_SistemaId = reader.GetInt32(3)
                });
            }

            return cursos;
        }

        public Curso? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Titulo, Descricao, Usuarios_SistemaId 
                           FROM Curso 
                           WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return new Curso
            {
                Id = reader.GetInt32(0),
                Titulo = reader.GetString(1),

                Descricao = reader.IsDBNull(2) ? null : reader.GetString(2),

                Usuarios_SistemaId = reader.GetInt32(3)
            };
        }

        public Curso Create(Curso curso)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                INSERT INTO Curso (Titulo, Descricao, Usuarios_SistemaId)
                VALUES (@Titulo, @Descricao, @Usuarios_SistemaId);
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Titulo", curso.Titulo);
            cmd.Parameters.AddWithValue("@Descricao", (object?)curso.Descricao ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Usuarios_SistemaId", curso.Usuarios_SistemaId);

            curso.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return curso;
        }

        public Curso? Update(int id, Curso curso)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                UPDATE Curso SET 
                    Titulo = @Titulo, 
                    Descricao = @Descricao, 
                    Usuarios_SistemaId = @Usuarios_SistemaId
                WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Titulo", curso.Titulo);
            cmd.Parameters.AddWithValue("@Descricao", (object?)curso.Descricao ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Usuarios_SistemaId", curso.Usuarios_SistemaId);
            cmd.Parameters.AddWithValue("@Id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows == 0) return null;

            curso.Id = id;
            return curso;
        }

        public bool Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"DELETE FROM Curso WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
