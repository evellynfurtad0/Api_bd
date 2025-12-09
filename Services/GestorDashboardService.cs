using Api_bd.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api_bd.Services
{
    public class GestorDashboardService : IGestorDashboardService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IInscricaoRepository _inscricaoRepo;
        private readonly ICursoRepository _cursoRepo;
        private readonly IDepartamentoRepository _departamentoRepo;
        private readonly IDepartamentoUsuarioRepository _departUsuarioRepo;
        private readonly IAtribuicaoCursoRepository _atribuicaoRepo;
        private readonly IProgressoCursoRepository _progressoRepo;

        public GestorDashboardService(
            IUsuarioRepository usuarioRepo,
            IInscricaoRepository inscricaoRepo,
            ICursoRepository cursoRepo,
            IDepartamentoRepository departamentoRepo,
            IDepartamentoUsuarioRepository departUsuarioRepo,
            IAtribuicaoCursoRepository atribuicaoRepo,
            IProgressoCursoRepository progressoRepo)
        {
            _usuarioRepo = usuarioRepo;
            _inscricaoRepo = inscricaoRepo;
            _cursoRepo = cursoRepo;
            _departamentoRepo = departamentoRepo;
            _departUsuarioRepo = departUsuarioRepo;
            _atribuicaoRepo = atribuicaoRepo;
            _progressoRepo = progressoRepo;
        }

        public List<Departamento> GetDepartamentosGerenciados(int gestorId)
        {
            return _departamentoRepo.GetAll()
                .Where(d => d.GestorId == gestorId)
                .ToList();
        }

        public (bool Success, string? Error) AdicionarUsuarioAoDepartamento(int departamentoId, int usuarioId)
        {
            var dep = _departamentoRepo.GetById(departamentoId);
            if (dep == null) return (false, "Departamento não encontrado.");

            var user = _usuarioRepo.GetById(usuarioId);
            if (user == null) return (false, "Usuário não encontrado.");

            bool ok = _departUsuarioRepo.AddUsuarioToDepartamento(usuarioId, departamentoId);
            if (!ok) return (false, "Usuário já pertence a esse departamento.");

            return (true, null);
        }

        public (bool Success, string? Error, int Inscritos) AtribuirCursoAoDepartamento(int gestorId, int cursoId, int departamentoId)
        {
            var curso = _cursoRepo.GetById(cursoId);
            if (curso == null) return (false, "Curso não encontrado.", 0);

            var dep = _departamentoRepo.GetById(departamentoId);
            if (dep == null) return (false, "Departamento não encontrado.", 0);

            var usuarios = _departUsuarioRepo.GetUsuariosByDepartamento(departamentoId);
            int inscritosCriados = 0;

            foreach (var u in usuarios)
            {
                bool ja = _inscricaoRepo.GetAll().Any(i => i.CursoId == cursoId && i.Usuarios_SistemaId == u.Id);

                if (!ja)
                {
                    _inscricaoRepo.Create(new Inscricao
                    {
                        CursoId = cursoId,
                        Usuarios_SistemaId = u.Id
                    });

                    inscritosCriados++;
                }
            }

            var atrib = new AtribuicaoCurso
            {
                GestorId = gestorId,
                CursoId = cursoId,
                DepartamentoId = departamentoId
            };

            _atribuicaoRepo.Create(atrib);

            return (true, null, inscritosCriados);
        }

        public List<Curso> GetCursosDoDepartamento(int departamentoId)
        {
            var usuarios = _departUsuarioRepo.GetUsuariosByDepartamento(departamentoId)
                .Select(u => u.Id)
                .ToList();

            var inscricoes = _inscricaoRepo.GetAll()
                .Where(i => usuarios.Contains(i.Usuarios_SistemaId))
                .ToList();

            var cursos = _cursoRepo.GetAll()
                .Where(c => inscricoes.Any(i => i.CursoId == c.Id))
                .Distinct()
                .ToList();

            return cursos;
        }

        public List<object> GetProgressoPorUsuarioDoDepartamento(int departamentoId)
        {
            var usuarios = _departUsuarioRepo.GetUsuariosByDepartamento(departamentoId);
            var progressos = _progressoRepo.GetAll();

            var resultado = new List<object>();

            foreach (var u in usuarios)
            {
                var pUser = progressos.Where(p => p.Usuarios_SistemaId == u.Id).ToList();
                int total = pUser.Count;
                int concluidos = pUser.Count(p => p.Status == "Concluído");
                double percentual = total == 0 ? 0 : (double)concluidos / total * 100;

                resultado.Add(new
                {
                    u.Id,
                    u.Nome,
                    Progresso = Math.Round(percentual, 2)
                });
            }

            return resultado;
        }

        public object GetMetricasPorDepartamentosGerenciados(int gestorId)
        {
            var deps = GetDepartamentosGerenciados(gestorId);

            var usuariosIds = deps
                .SelectMany(d => _departUsuarioRepo.GetUsuariosByDepartamento(d.Id).Select(u => u.Id))
                .Distinct()
                .ToList();

            var inscricoes = _inscricaoRepo.GetAll()
                .Where(i => usuariosIds.Contains(i.Usuarios_SistemaId))
                .ToList();

            var progressos = _progressoRepo.GetAll()
                .Where(p => usuariosIds.Contains(p.Usuarios_SistemaId))
                .ToList();

            int membros = usuariosIds.Count;
            int cursosEquipe = inscricoes.Select(i => i.CursoId).Distinct().Count();
            int concluidos = progressos.Count(p => p.Status == "Concluído");

            var medias = progressos
                .GroupBy(p => p.Usuarios_SistemaId)
                .Select(g =>
                {
                    int total = g.Count();
                    int done = g.Count(x => x.Status == "Concluído");
                    return total == 0 ? 0 : (double)done / total * 100;
                })
                .ToList();

            double progressoMedio = medias.Count == 0 ? 0 : medias.Average();

            return new
            {
                Membros = membros,
                CursosEquipe = cursosEquipe,
                ProgressoMedio = Math.Round(progressoMedio, 2),
                CursosConcluidos = concluidos
            };
        }
    }
}
