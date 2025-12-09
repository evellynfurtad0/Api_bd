using System;
using System.Collections.Generic;
using System.Linq;
using Api_bd.Repositories;

namespace Api_bd.Services
{
    public class DashboardFuncionarioService : IDashboardFuncionarioService
    {
        private readonly IInscricaoRepository _inscricaoRepo;
        private readonly ICursoRepository _cursoRepo;
        private readonly IProgressoCursoRepository _progressoRepo;
        private readonly IDepartamentoUsuarioRepository _departUsuarioRepo;
        private readonly IAtribuicaoCursoRepository _atribuicaoRepo;

        public DashboardFuncionarioService(
            IInscricaoRepository inscricaoRepo,
            ICursoRepository cursoRepo,
            IProgressoCursoRepository progressoRepo,
            IDepartamentoUsuarioRepository departUsuarioRepo,
            IAtribuicaoCursoRepository atribuicaoRepo)
        {
            _inscricaoRepo = inscricaoRepo;
            _cursoRepo = cursoRepo;
            _progressoRepo = progressoRepo;
            _departUsuarioRepo = departUsuarioRepo;
            _atribuicaoRepo = atribuicaoRepo;
        }

        public List<Curso> GetCursosEmAndamento(int usuarioId)
        {
            var andamento = _progressoRepo.GetAll()
                .Where(p => p.Usuarios_SistemaId == usuarioId &&
                            !string.Equals(p.Status, "Concluído", StringComparison.OrdinalIgnoreCase))
                .Select(p => p.CursoId)
                .Distinct()
                .ToList();

            var cursos = new List<Curso>();
            foreach (var id in andamento)
            {
                var c = _cursoRepo.GetById(id);
                if (c != null) cursos.Add(c);
            }

            return cursos;
        }

        public List<Curso> GetCursosDisponiveis(int usuarioId)
        {
            var inscritosIds = _inscricaoRepo.GetAll()
                .Where(i => i.Usuarios_SistemaId == usuarioId)
                .Select(i => i.CursoId)
                .ToHashSet();

            var departamentos = _departUsuarioRepo
                .GetDepartamentosDoUsuario(usuarioId)
                .Select(d => d.Id)
                .ToList();

            var cursosDep = _atribuicaoRepo.GetAll()
            .Where(a => a.DepartamentoId != null && departamentos.Contains(a.DepartamentoId.Value))
            .Select(a => a.CursoId)
            .Distinct()
            .ToList();


            return _cursoRepo.GetAll()
                .Where(c => cursosDep.Contains(c.Id) && !inscritosIds.Contains(c.Id))
                .ToList();
        }

        public (bool Success, string? Error) Inscrever(int usuarioId, int cursoId)
        {
            var jaInscrito = _inscricaoRepo.GetAll()
                .Any(i => i.CursoId == cursoId && i.Usuarios_SistemaId == usuarioId);

            if (jaInscrito)
                return (false, "Usuário já inscrito no curso.");

            var curso = _cursoRepo.GetById(cursoId);
            if (curso == null)
                return (false, "Curso não encontrado.");

            _inscricaoRepo.Create(new Inscricao
            {
                CursoId = cursoId,
                Usuarios_SistemaId = usuarioId
            });

            return (true, null);
        }

        public object GetMetricas(int usuarioId)
        {
            var inscricoes = _inscricaoRepo.GetAll()
                .Where(i => i.Usuarios_SistemaId == usuarioId)
                .ToList();

            var progressos = _progressoRepo.GetAll()
                .Where(p => p.Usuarios_SistemaId == usuarioId)
                .ToList();

            int matriculados = inscricoes.Count;
            int concluidos = progressos.Count(p =>
                string.Equals(p.Status, "Concluído", StringComparison.OrdinalIgnoreCase));

            int andamento = progressos.Count(p =>
                !string.Equals(p.Status, "Concluído", StringComparison.OrdinalIgnoreCase));

            var aulasDistintas = progressos
                .Select(p => p.AulaId)
                .Distinct()
                .Count();

            int horasEstimadas = aulasDistintas;

            return new
            {
                CursosMatriculados = matriculados,
                CursosConcluidos = concluidos,
                CursosEmAndamento = andamento,
                HorasEstudo = horasEstimadas
            };
        }
    }
}
