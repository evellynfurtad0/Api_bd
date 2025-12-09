public class CursoService : ICursoService
{
    //repositorios sendo usados para acessar o banco de dados
    private readonly ICursoRepository _cursoRepo;
    private readonly IModuloRepository _moduloRepo;
    private readonly IAulaRepository _aulaRepo;

    public CursoService(
        ICursoRepository cursoRepo,
        IModuloRepository moduloRepo,
        IAulaRepository aulaRepo)
    {
        //IJ
        _cursoRepo = cursoRepo;
        _moduloRepo = moduloRepo;
        _aulaRepo = aulaRepo;
    }

    //validações
    public (bool Success, string? Error, Curso? Curso) CriarCursoCompleto(CreateCursoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
            return (false, "O título do curso é obrigatório.", null);

        if (dto.Modulos.Count > 30)
            return (false, "Um curso pode ter no máximo 30 módulos.", null);

        // Cria o Curso no banco
        var curso = _cursoRepo.Create(new Curso
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Usuarios_SistemaId = dto.Usuarios_SistemaId
        });

        foreach (var mod in dto.Modulos)
        {
            if (mod.Aulas.Count > 100)
                return (false, $"O módulo '{mod.Titulo}' excede o limite de 100 aulas.", null);

            // Cria o módulo no banco
            var moduloCriado = _moduloRepo.Create(new Modulo
            {
                Titulo = mod.Titulo,
                Ordem = mod.Ordem,
                CursoId = curso.Id
            });

            // Cria aulas do
            foreach (var aula in mod.Aulas)
            {
                _aulaRepo.Create(new Aula
                {
                    Titulo = aula.Titulo,
                    Conteudo = aula.Conteudo,     
                    ChaveVideo = aula.ChaveVideo, 
                    Ordem = aula.Ordem,
                    ModuloId = moduloCriado.Id
                });
            }
        }

        return (true, null, curso);
    }
}
