using ChessPlatform.Entities;
using System.Collections.Generic;

namespace ChessPlatform.Repositories
{
    public interface IGameRepository : IRepository
    {
        void Add(Game game);
        void Update(Game game);
        void Delete(Game game);

        Game Get(int id);

        ICollection<Game> GetGames();
        ICollection<Game> GetNewGames();
        ICollection<Game> GetGameInProgress();
        ICollection<Game> GetEndedGames();
        ICollection<Game> GetGamesByPlayer(string playerId);
        ICollection<Game> GetGamesByPoints(int? minimumNumberOfPoints, int? maximumNumberOfPoints);
    }
}
