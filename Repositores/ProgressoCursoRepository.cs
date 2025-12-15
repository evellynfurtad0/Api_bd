using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Api_bd.Repositories
{
    public class ProgressoCursoRepository : IProgressoCursoRepository
    {
        private readonly string _connectionString;

        public ProgressoCursoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }

        public List<ProgressoCurso> GetAll()
        {
            List<ProgressoCurso> progressos = new();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Usuarios_SistemaId, CursoId, ModuloId, AulaId, Status 
                           FROM ProgressoCurso";

            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                progressos.Add(new ProgressoCurso
                {
                    Id = reader.GetInt32(0),
                    Usuarios_SistemaId = reader.GetInt32(1),
                    CursoId = reader.GetInt32(2),
                    ModuloId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    AulaId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    Status = reader.GetString(5)
                });
            }

            return progressos;
        }

        public ProgressoCurso? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Usuarios_SistemaId, CursoId, ModuloId, AulaId, Status 
                           FROM ProgressoCurso 
                           WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return new ProgressoCurso
            {
                Id = reader.GetInt32(0),
                Usuarios_SistemaId = reader.GetInt32(1),
                CursoId = reader.GetInt32(2),
                ModuloId = reader.GetInt32(3),
                AulaId = reader.GetInt32(4),
                Status = reader.GetString(5)
            };
        }

        public ProgressoCurso Create(ProgressoCurso progresso)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                INSERT INTO ProgressoCurso 
                (Usuarios_SistemaId, CursoId, ModuloId, AulaId, Status)
                VALUES 
                (@Usuarios_SistemaId, @CursoId, @ModuloId, @AulaId, @Status);
                SELECT SCOPE_IDENTITY();
            ";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add("@Usuarios_SistemaId", SqlDbType.Int)
                .Value = progresso.Usuarios_SistemaId;

            cmd.Parameters.Add("@CursoId", SqlDbType.Int)
                .Value = progresso.CursoId;

            cmd.Parameters.Add("@ModuloId", SqlDbType.Int)
                .Value = (object?)progresso.ModuloId ?? DBNull.Value;

            cmd.Parameters.Add("@AulaId", SqlDbType.Int)
                .Value = (object?)progresso.AulaId ?? DBNull.Value;

            cmd.Parameters.Add("@Status", SqlDbType.VarChar)
                .Value = progresso.Status;

            progresso.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return progresso;
        }

        public ProgressoCurso? Update(int id, ProgressoCurso progresso)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                UPDATE ProgressoCurso SET
                    Usuarios_SistemaId = @Usuarios_SistemaId,
                    CursoId = @CursoId,
                    ModuloId = @ModuloId,
                    AulaId = @AulaId,
                    Status = @Status
                WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Usuarios_SistemaId", progresso.Usuarios_SistemaId);
            cmd.Parameters.AddWithValue("@CursoId", progresso.CursoId);
            cmd.Parameters.AddWithValue("@ModuloId", progresso.ModuloId);
            cmd.Parameters.AddWithValue("@AulaId", progresso.AulaId);
            cmd.Parameters.AddWithValue("@Status", progresso.Status);
            cmd.Parameters.AddWithValue("@Id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows == 0) return null;

            progresso.Id = id;
            return progresso;
        }

        public bool Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"DELETE FROM ProgressoCurso WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
