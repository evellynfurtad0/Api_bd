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
        private readonly IModuloRepository _moduloRepo;
        private readonly IAulaRepository _aulaRepo;

        public DashboardFuncionarioService(
            IInscricaoRepository inscricaoRepo,
            ICursoRepository cursoRepo,
            IProgressoCursoRepository progressoRepo,
            IDepartamentoUsuarioRepository departUsuarioRepo,
            IAtribuicaoCursoRepository atribuicaoRepo,
            IModuloRepository moduloRepo,
            IAulaRepository aulaRepo)
        {
            _inscricaoRepo = inscricaoRepo;
            _cursoRepo = cursoRepo;
            _progressoRepo = progressoRepo;
            _departUsuarioRepo = departUsuarioRepo;
            _atribuicaoRepo = atribuicaoRepo;
            _moduloRepo = moduloRepo;
            _aulaRepo = aulaRepo;
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

        public (bool Success, string? Error, object? Data) GetConteudoCurso(int usuarioId, int cursoId)
        {
            var inscrito = _inscricaoRepo.GetAll()
                .Any(i => i.Usuarios_SistemaId == usuarioId && i.CursoId == cursoId);

            if (!inscrito)
                return (false, "Usuário não está inscrito no curso.", null);

            var curso = _cursoRepo.GetById(cursoId);
            if (curso == null)
                return (false, "Curso não encontrado.", null);

            var modulos = _moduloRepo.GetByCursoId(cursoId);
            var aulas = _aulaRepo.GetAll();

            var data = new
            {
                CursoId = curso.Id,
                Titulo = curso.Titulo,
                Descricao = curso.Descricao,
                Modulos = modulos
                    .OrderBy(m => m.Ordem)
                    .Select(m => new
                    {
                        ModuloId = m.Id,
                        Titulo = m.Titulo,
                        Ordem = m.Ordem,
                        Aulas = aulas
                            .Where(a => a.ModuloId == m.Id)
                            .OrderBy(a => a.Ordem)
                            .Select(a => new
                            {
                                AulaId = a.Id,
                                Titulo = a.Titulo,
                                Conteudo = a.Conteudo,                                ChaveVideo = a.ChaveVideo,
                                Ordem = a.Ordem
                            })
                    })
            };

            return (true, null, data);
        }
        
        public (bool Success, string? Error) IniciarCurso(int usuarioId, int cursoId)
        {
            // Verifica se está inscrito
            var inscrito = _inscricaoRepo.GetAll()
                .Any(i => i.Usuarios_SistemaId == usuarioId && i.CursoId == cursoId);

            if (!inscrito)
                return (false, "Usuário não está inscrito neste curso.");

            //Verifica se já existe progresso
            var jaIniciado = _progressoRepo.GetAll()
                .Any(p => p.Usuarios_SistemaId == usuarioId && p.CursoId == cursoId);

            if (jaIniciado)
                return (false, "Curso já iniciado.");

            //Cria progresso 
            _progressoRepo.Create(new ProgressoCurso
            {
                Usuarios_SistemaId = usuarioId,
                CursoId = cursoId,
                ModuloId = null,
                AulaId = null,
                Status = "Em andamento"
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

            int cursosMatriculados = inscricoes
                .Select(i => i.CursoId)
                .Distinct()
                .Count();

            int cursosConcluidos = progressos
                .Where(p => p.Status.Equals("Concluído", StringComparison.OrdinalIgnoreCase))
                .Select(p => p.CursoId)
                .Distinct()
                .Count();

            int cursosEmAndamento = cursosMatriculados - cursosConcluidos;

            return new
            {
                CursosMatriculados = cursosMatriculados,
                CursosConcluidos = cursosConcluidos,
                CursosEmAndamento = cursosEmAndamento
            };
        }
    }
}