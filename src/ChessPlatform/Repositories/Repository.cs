using ChessPlatform.DBContexts;
using System;

namespace ChessPlatform.Repositories
{
    public class Repository : IRepository
    {
        private readonly ChessContext _context;

        public Repository(ChessContext context)
        {
            _context = context;
        }

        public void RollBackChanges()
        {
            throw new NotImplementedException();
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
