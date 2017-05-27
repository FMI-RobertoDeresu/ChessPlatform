using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessPlatform.Entities
{
    public class Game
    {
        ///// <summary>
        ///// This must be temporary.
        ///// </summary>
        private const string DefaultStatus = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public int Id { get; protected set; }
        public string Player1Id { get; protected set; }
        public string Player2Id { get; protected set; }
        public string WinnerId { get; protected set; }

        public DateTime CreatedAt { get; protected set; }
        public DateTime? StartedAt { get; protected set; }
        public DateTime? EndedAt { get; protected set; }
        public string Name { get; protected set; }
        public string Password { get; protected set; }
        public bool AllowSpectate { get; protected set; }
        public int? MinimumNumberOfPoints { get; protected set; }
        public int? MaximumNumberOfPoints { get; protected set; }
        public int Turn { get; protected set; }
        public string Status { get; protected set; }
        public string Moves { get; protected set; }
        public bool WithComputer { get; protected set; }

        public virtual User Player1 { get; protected set; }
        public virtual User Player2 { get; protected set; }
        public virtual User Winner { get; protected set; }

        protected Game() { }

        public Game(string name, User player, bool allowSpectate, string password, int? minimumNumberOfPoints, int? maximumNumberOfPoints, bool withComputer)
        {
            CreatedAt = DateTime.Now;
            StartedAt = null;
            EndedAt = null;
            Name = name;
            Player1 = player;
            Player2 = null;
            Winner = null;
            AllowSpectate = allowSpectate;
            Password = password;
            MinimumNumberOfPoints = minimumNumberOfPoints;
            MaximumNumberOfPoints = maximumNumberOfPoints;
            Moves = null;
            Turn = 0;
            Status = DefaultStatus;
            WithComputer = withComputer;
        }

        public void StartGame(User player)
        {
            StartedAt = DateTime.Now;
            Player2 = player;
        }

        public void EndGame(bool isCheckmate)
        {
            EndedAt = DateTime.Now;

            if (isCheckmate)
            {
                if (Turn % 2 == 1)
                {
                    Winner = Player1;
                    Player1.Profile.GamesWon++;

                    if (!WithComputer)
                    {
                        Player2.Profile.GamesLosses++;
                    }
                }
                else
                {
                    Winner = Player2;
                    Player1.Profile.GamesLosses++;

                    if (!WithComputer)
                    {
                        Player2.Profile.GamesWon++;
                    }
                }
            }
            else
            {
                Player1.Profile.GamesDraw++;

                if (!WithComputer)
                {
                    Player2.Profile.GamesDraw++;
                }
            }
        }

        public IList<string> GetMoves()
        {
            return Moves.Split(';').ToList();
        }

        public void AddMove(string from, string to, string promotion, string status)
        {
            Moves = Moves + from + '-' + to + '_' + promotion + ';';
            Status = status;
            Turn++;
        }
    }
}