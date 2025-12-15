public interface IModuloRepository
{
    List<Modulo> GetAll();
    Modulo? GetById(int id);
    Modulo Create(Modulo modulo);
    Modulo? Update(int id, Modulo modulo);
    bool Delete(int id);
    List<Modulo> GetByCursoId(int cursoId);
}
