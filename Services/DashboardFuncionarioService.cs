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

        // Cursos que o funcionário está inscrito
        public List<Curso> GetCursosEmAndamento(int usuarioId)
        {
            var inscritosCursoIds = _inscricaoRepo.GetAll()
                .Where(i => i.Usuarios_SistemaId == usuarioId)
                .Select(i => i.CursoId)
                .Distinct()
                .ToList();

            return inscritosCursoIds
                .Select(id => _cursoRepo.GetById(id))
                .Where(c => c != null)
                .Select(c => c!)
                .ToList();
        }

        public List<Curso> GetCursosDisponiveis(int usuarioId)
        {
            // Cursos já inscritos
            var inscritosIds = _inscricaoRepo.GetAll()
                .Where(i => i.Usuarios_SistemaId == usuarioId)
                .Select(i => i.CursoId)
                .ToHashSet();

            var departamentos = _departUsuarioRepo.GetDepartamentosDoUsuario(usuarioId);

            if (departamentos == null || departamentos.Count == 0)
                return new List<Curso>();

            var depIds = departamentos.Select(d => d.Id).ToHashSet();

            var atribuicoes = _atribuicaoRepo.GetAll()
                .Where(a => a.DepartamentoId != null)
                .ToList();

            var cursosDoDepartamento = atribuicoes
                .Where(a => depIds.Contains(a.DepartamentoId!.Value))
                .Select(a => a.CursoId)
                .Distinct()
                .ToList();

            if (cursosDoDepartamento.Count == 0)
                return new List<Curso>();

            var disponiveis = cursosDoDepartamento
                .Where(id => !inscritosIds.Contains(id))
                .Select(id => _cursoRepo.GetById(id))
                .Where(c => c != null)
                .Select(c => c!)
                .ToList();

            return disponiveis;
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

            int cursosConcluidos = progressos
                .GroupBy(p => p.CursoId)
                .Count(g => g.All(p =>
                    string.Equals(p.Status, "Concluído", StringComparison.OrdinalIgnoreCase)));

            int cursosEmAndamento = Math.Max(0, matriculados - cursosConcluidos);

            int horasEstimadas = progressos
                .Where(p => string.Equals(p.Status, "Concluído", StringComparison.OrdinalIgnoreCase))
                .Select(p => p.AulaId)
                .Distinct()
                .Count();

            return new
            {
                CursosMatriculados = matriculados,
                CursosConcluidos = cursosConcluidos,
                CursosEmAndamento = cursosEmAndamento,
                HorasEstudo = horasEstimadas
            };
        }
    }
}