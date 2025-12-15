using System.Collections.Generic;

namespace Api_bd.Services
{
    public interface IDashboardFuncionarioService
    {
        List<Curso> GetCursosEmAndamento(int usuarioId);
        List<Curso> GetCursosDisponiveis(int usuarioId);
        (bool Success, string? Error) Inscrever(int usuarioId, int cursoId);
        (bool Success, string? Error) IniciarCurso(int usuarioId, int cursoId);
        object GetMetricas(int usuarioId);
    }
}
