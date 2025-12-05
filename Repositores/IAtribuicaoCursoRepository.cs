public interface IAtribuicaoCursoRepository
{
    List<AtribuicaoCurso> GetAll();
    AtribuicaoCurso? GetById(int id);
    AtribuicaoCurso Create(AtribuicaoCurso atrib);
    AtribuicaoCurso? Update(int id, AtribuicaoCurso atrib);
    bool Delete(int id);
}
