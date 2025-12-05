using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Api_bd.Repositories
{
    public class AtribuicaoCursoRepository : IAtribuicaoCursoRepository
    {
        private readonly string _connectionString;

        public AtribuicaoCursoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }

        public List<AtribuicaoCurso> GetAll()
        {
            List<AtribuicaoCurso> atribuicoes = new();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, GestorId, CursoId 
                           FROM AtribuicaoCurso";

            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                atribuicoes.Add(new AtribuicaoCurso
                {
                    Id = reader.GetInt32(0),
                    GestorId = reader.GetInt32(1),
                    CursoId = reader.GetInt32(2)
                });
            }

            return atribuicoes;
        }

        public AtribuicaoCurso? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, GestorId, CursoId 
                           FROM AtribuicaoCurso 
                           WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return new AtribuicaoCurso
            {
                Id = reader.GetInt32(0),
                GestorId = reader.GetInt32(1),
                CursoId = reader.GetInt32(2)
            };
        }

        public AtribuicaoCurso Create(AtribuicaoCurso atrib)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                INSERT INTO AtribuicaoCurso (GestorId, CursoId)
                VALUES (@GestorId, @CursoId);
                SELECT SCOPE_IDENTITY();
            ";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@GestorId", atrib.GestorId);
            cmd.Parameters.AddWithValue("@CursoId", atrib.CursoId);

            atrib.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return atrib;
        }

        public AtribuicaoCurso? Update(int id, AtribuicaoCurso atrib)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                UPDATE AtribuicaoCurso SET 
                    GestorId = @GestorId,
                    CursoId = @CursoId
                WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@GestorId", atrib.GestorId);
            cmd.Parameters.AddWithValue("@CursoId", atrib.CursoId);
            cmd.Parameters.AddWithValue("@Id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows == 0) return null;

            atrib.Id = id;
            return atrib;
        }

        public bool Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"DELETE FROM AtribuicaoCurso WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}