using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Api_bd.Repositories
{
    public class AulaRepository : IAulaRepository
    {
        private readonly string _connectionString;

        public AulaRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }

        public List<Aula> GetAll()
        {
            List<Aula> aulas = new();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Titulo, Conteudo, ChaveVideo, Ordem, ModuloId 
                           FROM Aula";

            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                aulas.Add(new Aula
                {
                    Id = reader.GetInt32(0),
                    Titulo = reader.GetString(1),
                    Conteudo = reader.GetString(2),
                    ChaveVideo = reader.GetString(3),
                    Ordem = reader.GetInt32(4),
                    ModuloId = reader.GetInt32(5)
                });
            }

            return aulas;
        }

        public Aula? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Titulo, Conteudo, ChaveVideo, Ordem, ModuloId 
                           FROM Aula WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return new Aula
            {
                Id = reader.GetInt32(0),
                Titulo = reader.GetString(1),
                Conteudo = reader.GetString(2),
                ChaveVideo = reader.GetString(3),
                Ordem = reader.GetInt32(4),
                ModuloId = reader.GetInt32(5)
            };
        }

        public Aula Create(Aula aula)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                INSERT INTO Aula (Titulo, Conteudo, ChaveVideo, Ordem, ModuloId)
                VALUES (@Titulo, @Conteudo, @ChaveVideo, @Ordem, @ModuloId);
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Titulo", aula.Titulo);
            cmd.Parameters.AddWithValue("@Conteudo", aula.Conteudo);
            cmd.Parameters.AddWithValue("@ChaveVideo", aula.ChaveVideo);
            cmd.Parameters.AddWithValue("@Ordem", aula.Ordem);
            cmd.Parameters.AddWithValue("@ModuloId", aula.ModuloId);

            aula.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return aula;
        }

        public Aula? Update(int id, Aula aula)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                UPDATE Aula SET 
                    Titulo = @Titulo,
                    Conteudo = @Conteudo,
                    ChaveVideo = @ChaveVideo,
                    Ordem = @Ordem,
                    ModuloId = @ModuloId
                WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Titulo", aula.Titulo);
            cmd.Parameters.AddWithValue("@Conteudo", aula.Conteudo);
            cmd.Parameters.AddWithValue("@ChaveVideo", aula.ChaveVideo);
            cmd.Parameters.AddWithValue("@Ordem", aula.Ordem);
            cmd.Parameters.AddWithValue("@ModuloId", aula.ModuloId);
            cmd.Parameters.AddWithValue("@Id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows == 0) return null;

            aula.Id = id;
            return aula;
        }

        public bool Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"DELETE FROM Aula WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}