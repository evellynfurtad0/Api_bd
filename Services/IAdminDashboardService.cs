using System.Collections.Generic;

namespace Api_bd.Services
{
    public interface IAdminDashboardService
    {
        List<Curso> GetCursosRecentes();
        List<Departamento> GetTodasEquipes();
        object GetMetricasGerais();
        object GetDepartamentosComUsuarios();
        (bool Success, string? Error) PromoverParaGestor(int usuarioId);
        (bool Success, string? Error, Departamento? Departamento) CriarDepartamento(Departamento dep);
    }
}
