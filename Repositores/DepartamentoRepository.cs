using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Api_bd.Repositories
{
    public interface IDepartamentoRepository
    {
        List<Departamento> GetAll();
        Departamento? GetById(int id);
    }

    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly string _connectionString;

        public DepartamentoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }

        public List<Departamento> GetAll()
        {
            var list = new List<Departamento>();
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Nome, Descricao, GestorId FROM Departamento";
            using var cmd = new SqlCommand(sql, con);
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

        public Departamento? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string sql = @"SELECT Id, Nome, Descricao, GestorId FROM Departamento WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Departamento
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descricao = reader.IsDBNull(2) ? null : reader.GetString(2),
                GestorId = reader.IsDBNull(3) ? null : reader.GetInt32(3)
            };
        }
    }
}
