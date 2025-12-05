using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Api_bd.Repositories
{
    public class ModuloRepository : IModuloRepository
    {
        private readonly string _connectionString;

        public ModuloRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }

        public List<Modulo> GetAll()
        {
            List<Modulo> modulos = new();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Titulo, Ordem, CursoId 
                           FROM Modulo";

            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                modulos.Add(new Modulo
                {
                    Id = reader.GetInt32(0),
                    Titulo = reader.GetString(1),
                    Ordem = reader.GetInt32(2),
                    CursoId = reader.GetInt32(3)
                });
            }

            return modulos;
        }

        public Modulo? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Titulo, Ordem, CursoId 
                           FROM Modulo 
                           WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return new Modulo
            {
                Id = reader.GetInt32(0),
                Titulo = reader.GetString(1),
                Ordem = reader.GetInt32(2),
                CursoId = reader.GetInt32(3)
            };
        }

        public Modulo Create(Modulo modulo)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                INSERT INTO Modulo (Titulo, Ordem, CursoId)
                VALUES (@Titulo, @Ordem, @CursoId);
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Titulo", modulo.Titulo);
            cmd.Parameters.AddWithValue("@Ordem", modulo.Ordem);
            cmd.Parameters.AddWithValue("@CursoId", modulo.CursoId);

            modulo.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return modulo;
        }

        public Modulo? Update(int id, Modulo modulo)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"
                UPDATE Modulo SET
                    Titulo = @Titulo,
                    Ordem = @Ordem,
                    CursoId = @CursoId
                WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Titulo", modulo.Titulo);
            cmd.Parameters.AddWithValue("@Ordem", modulo.Ordem);
            cmd.Parameters.AddWithValue("@CursoId", modulo.CursoId);
            cmd.Parameters.AddWithValue("@Id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows == 0) return null;

            modulo.Id = id;
            return modulo;
        }

        public bool Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"DELETE FROM Modulo WHERE Id = @Id";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
