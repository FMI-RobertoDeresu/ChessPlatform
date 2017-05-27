namespace ChessPlatform.Repositories
{
    public interface IRepository
    {
        void RollBackChanges();
        bool SaveChanges();
    }
}
