using System.Collections.Generic;
using System.Linq;
using ChessPlatform.DBContexts;
using ChessPlatform.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChessPlatform.Repositories
{
    public class GameRepository : Repository, IGameRepository
    {
        private readonly DbSet<Game> _dbSet;

        public GameRepository(ChessContext context) : base(context)
        {
            _dbSet = context.Set<Game>();
        }

        private IQueryable<Game> Games
        {
            get
            {
                return _dbSet
                    .Include(x => x.Player1)
                    .Include(x => x.Player2)
                    .Include(x => x.Winner);
            }
        }

        private IQueryable<Game> NewGames
        {
            get { return Games.Where(x => x.StartedAt == null); }
        }

        private IQueryable<Game> GamesInProgress
        {
            get { return Games.Where(x => x.StartedAt.HasValue && !x.EndedAt.HasValue); }
        }

        private IQueryable<Game> EndedGames
        {
            get { return Games.Where(x => x.EndedAt.HasValue); }
        }

        public void Add(Game game)
        {
            _dbSet.Add(game);
        }

        public void Update(Game game)
        {
            _dbSet.Update(game);
        }

        public void Delete(Game game)
        {
            _dbSet.Remove(game);
        }

        public Game Get(int id)
        {
            return Games.Where(x => x.Id == id).FirstOrDefault();
        }

        public ICollection<Game> GetGames()
        {
            return Games.ToList();
        }

        public ICollection<Game> GetNewGames()
        {
            return NewGames.ToList();
        }

        public ICollection<Game> GetGameInProgress()
        {
            return GamesInProgress.ToList();
        }

        public ICollection<Game> GetEndedGames()
        {
            return EndedGames.ToList();
        }

        public ICollection<Game> GetGamesByPlayer(string playerId)
        {
            return Games.Where(x => x.Player1.Id == playerId).ToList();
        }

        public ICollection<Game> GetGamesByPoints(int? minimumNumberOfPoints, int? maximumNumberOfPoints)
        {
            return null;
            // Games.Where(x => (minimumNumberOfPoints == null || minimumNumberOfPoints <= x.Player1.Profile.Points) && (maximumNumberOfPoints == null || x.Player1.Profile.Points <= maximumNumberOfPoints)).ToList();
        }
    }
}