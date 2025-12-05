namespace Api_bd.Repositories{  
    public interface IProgressoCursoRepository
    {
        List<ProgressoCurso> GetAll();
        ProgressoCurso? GetById(int id);
        ProgressoCurso Create(ProgressoCurso progresso);
        ProgressoCurso? Update(int id, ProgressoCurso progresso);
        bool Delete(int id);
    }
}
