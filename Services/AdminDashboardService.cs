using Api_bd.Repositories;
using System.Linq;

namespace Api_bd.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ICursoRepository _cursoRepo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IDepartamentoRepository _depRepo;
        private readonly IProgressoCursoRepository _progressoRepo;
        private readonly IDepartamentoUsuarioRepository _depUsuarioRepo;

        public AdminDashboardService(
            ICursoRepository cursoRepo,
            IUsuarioRepository usuarioRepo,
            IDepartamentoRepository depRepo,
            IDepartamentoUsuarioRepository depUsuarioRepo,
            IProgressoCursoRepository progressoRepo)
        {
            _cursoRepo = cursoRepo;
            _usuarioRepo = usuarioRepo;
            _depRepo = depRepo;
            _progressoRepo = progressoRepo;
            _depUsuarioRepo = depUsuarioRepo;
        }

        public List<Curso> GetCursosRecentes()
        {
            return _cursoRepo.GetAll()
                .OrderByDescending(c => c.Id)
                .Take(10)
                .ToList();
        }

        public List<Departamento> GetTodasEquipes()
        {
            return _depRepo.GetAll();
        }

        public object GetMetricasGerais()
        {
            var totalCursos = _cursoRepo.GetAll().Count();
            var totalUsuarios = _usuarioRepo.GetAll().Count();
            var totalEquipes = _depRepo.GetAll().Count();

            var progresso = _progressoRepo.GetAll();
            var totalRegistros = progresso.Count();
            var concluidos = progresso.Count(p => p.Status == "Concluído");

            double taxaConclusao = totalRegistros == 0
                ? 0
                : (double)concluidos / totalRegistros * 100;

            return new
            {
                TotalCursos = totalCursos,
                TotalUsuarios = totalUsuarios,
                TotalEquipes = totalEquipes,
                TaxaConclusaoGeral = Math.Round(taxaConclusao, 2)
            };
        }

        public (bool Success, string? Error) PromoverParaGestor(int usuarioId)
        {
            var usuario = _usuarioRepo.GetById(usuarioId);
            if (usuario == null)
                return (false, "Usuário não encontrado.");

            usuario.Perfil = PerfilEnum.Gestor;

            var updated = _usuarioRepo.Update(usuarioId, usuario);
            if (updated == null)
                return (false, "Erro ao atualizar usuário.");

            return (true, null);
        }

        public (bool Success, string? Error, Departamento? Departamento) CriarDepartamento(Departamento dep)
        {
            if (string.IsNullOrWhiteSpace(dep.Nome))
                return (false, "Nome é obrigatório.", null);

            var criado = _depRepo.Create(dep);
            return (true, null, criado);
        }

        public object GetDepartamentosComUsuarios()
        {
            var deps = _depRepo.GetAll();
            var result = new List<object>();

            foreach (var d in deps)
            {
                var usuarios = _depUsuarioRepo.GetUsuariosByDepartamento(d.Id);

                result.Add(new 
                {
                    DepartamentoId = d.Id,
                    DepartamentoNome = d.Nome,
                    Usuarios = usuarios.Select(u => new { u.Id, u.Nome, u.Email })
                });
            }

            return result;
        }
    }
}
