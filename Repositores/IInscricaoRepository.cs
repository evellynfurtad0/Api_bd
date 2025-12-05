public interface IInscricaoRepository
{
    List<Inscricao> GetAll();
    Inscricao? GetById(int id);
    Inscricao Create(Inscricao inscricao);
    Inscricao? Update(int id, Inscricao inscricao);
    bool Delete(int id);
}
