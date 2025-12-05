using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Api_bd.Repositories
{
    public class InscricaoRepository : IInscricaoRepository
    {
        private readonly string _connectionString;

        public InscricaoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }

        public List<Inscricao> GetAll()
        {
            List<Inscricao> inscricoes = new();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Usuarios_SistemaId, CursoId 
                           FROM Inscricao";

            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                inscricoes.Add(new Inscricao
                {
                    Id = reader.GetInt32(0),
                    Usuarios_SistemaId = reader.GetInt32(1),
                    CursoId = reader.GetInt32(2)
                });
            }

            return inscricoes;
        }

        public Inscricao? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Usuarios_SistemaId, CursoId 
                           FROM Inscricao WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return new Inscricao
            {
                Id = reader.GetInt32(0),
                Usuarios_SistemaId = reader.GetInt32(1),
                CursoId = reader.GetInt32(2)
            };
        }

        public Inscricao Create(Inscricao inscricao)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                INSERT INTO Inscricao (Usuarios_SistemaId, CursoId)
                VALUES (@Usuarios_SistemaId, @CursoId);
                SELECT SCOPE_IDENTITY();
            ";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Usuarios_SistemaId", inscricao.Usuarios_SistemaId);
            cmd.Parameters.AddWithValue("@CursoId", inscricao.CursoId);

            inscricao.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return inscricao;
        }

        public Inscricao? Update(int id, Inscricao inscricao)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                UPDATE Inscricao SET
                    Usuarios_SistemaId = @Usuarios_SistemaId,
                    CursoId = @CursoId
                WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Usuarios_SistemaId", inscricao.Usuarios_SistemaId);
            cmd.Parameters.AddWithValue("@CursoId", inscricao.CursoId);
            cmd.Parameters.AddWithValue("@Id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows == 0) return null;

            inscricao.Id = id;
            return inscricao;
        }

        public bool Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"DELETE FROM Inscricao WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}