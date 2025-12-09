public interface ICursoService
{
    (bool Success, string? Error, Curso? Curso) CriarCursoCompleto(CreateCursoDto dto);
}
