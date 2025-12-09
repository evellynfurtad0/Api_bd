using System.Collections.Generic;

namespace Api_bd.Services
{
    public interface IGestorDashboardService
    {
        List<Departamento> GetDepartamentosGerenciados(int gestorId);

        (bool Success, string? Error) AdicionarUsuarioAoDepartamento(int departamentoId, int usuarioId);

        (bool Success, string? Error, int Inscritos) AtribuirCursoAoDepartamento(int gestorId, int cursoId, int departamentoId);

        List<Curso> GetCursosDoDepartamento(int departamentoId);

        List<object> GetProgressoPorUsuarioDoDepartamento(int departamentoId);

        object GetMetricasPorDepartamentosGerenciados(int gestorId);
    }
}
