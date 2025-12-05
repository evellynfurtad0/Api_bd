public interface ICursoRepository
{
    List<Curso> GetAll();
    Curso? GetById(int id);
    Curso Create(Curso curso);
    Curso? Update(int id, Curso curso);
    bool Delete(int id);
}
