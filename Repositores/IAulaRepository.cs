public interface IAulaRepository
{
    List<Aula> GetAll();
    Aula? GetById(int id);
    Aula Create(Aula aula);
    Aula? Update(int id, Aula aula);
    bool Delete(int id);
}
