using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessPlatform.DBContexts;
using ChessPlatform.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChessPlatform.Repositories
{
    public class UserRepository : Repository, IUserRepository
    {
        private readonly ChessContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(ChessContext context, UserManager<User> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        private IQueryable<User> Users
        {
            get { return _context.Users; }
        }

        private IQueryable<User> UsersWithProfile
        {
            get { return Users.Include(x => x.Profile); }
        }

        private IQueryable<User> UsersWithGames
        {
            get
            {
                return Users.Include(x => x.GamesAsPlayer1)
                    .Include(x => x.GamesAsPlayer2)
                    .Include(x => x.GamesAsWinner);
            }
        }

        private IQueryable<User> UsersWithAllData
        {
            get
            {
                return Users.Include(x => x.Profile)
                    .Include(x => x.GamesAsPlayer1)
                    .Include(x => x.GamesAsPlayer2)
                    .Include(x => x.GamesAsWinner)
                    .Include(x => x.Notifications);
            }
        }


        public Task<IdentityResult> Add(User newUser, string password)
        {
            return _userManager.CreateAsync(newUser, password);
        }

        public void Update(User user)
        {
            _userManager.UpdateAsync(user).Wait();
        }

        public void Delete(User user)
        {
            _userManager.DeleteAsync(user).Wait();
        }


        public User GetById(string id)
        {
            return Users.Where(x => x.Id == id).FirstOrDefault();
        }

        public User GetByExternalId(int externalId)
        {
            return Users.Where(x => x.ExternalId == externalId).FirstOrDefault();
        }

        public User GetByUsername(string username)
        {
            return Users.Where(x => x.UserName == username).FirstOrDefault();
        }

        public User GetByEmail(string email)
        {
            return Users.Where(x => x.Email == email).FirstOrDefault();
        }


        public User GetWithProfileById(string id)
        {
            return UsersWithProfile.Where(x => x.Id == id).FirstOrDefault();
        }

        public User GetWithProfileByExternalId(int externalId)
        {
            return UsersWithProfile.Where(x => x.ExternalId == externalId).FirstOrDefault();
        }

        public User GetWithProfileByUsername(string username)
        {
            return UsersWithProfile.Where(x => x.UserName == username).FirstOrDefault();
        }

        public User GetWithProfileByEmail(string email)
        {
            return UsersWithProfile.Where(x => x.Email == email).FirstOrDefault();
        }


        public User GetWithGamesById(string id)
        {
            return UsersWithGames.Where(x => x.Id == id).FirstOrDefault();
        }

        public User GetWithGamesByExternalId(int externalId)
        {
            return UsersWithGames.Where(x => x.ExternalId == externalId).FirstOrDefault();
        }

        public User GetWithGamesByUsername(string username)
        {
            return UsersWithGames.Where(x => x.UserName == username).FirstOrDefault();
        }

        public User GetWithGamesByEmail(string email)
        {
            return UsersWithGames.Where(x => x.Email == email).FirstOrDefault();
        }


        public User GetWithAllDataById(string id)
        {
            return UsersWithAllData.Where(x => x.Id == id).FirstOrDefault();
        }

        public User GetWithAllDataByExternalId(int externalId)
        {
            return UsersWithAllData.Where(x => x.ExternalId == externalId).FirstOrDefault();
        }

        public User GetWithAllDataByUsername(string username)
        {
            return UsersWithAllData.Where(x => x.UserName == username).FirstOrDefault();
        }

        public User GetWithAllDataByEmail(string email)
        {
            return UsersWithAllData.Where(x => x.Email == email).FirstOrDefault();
        }


        public ICollection<User> GetAll()
        {
            return Users.ToList();
        }

        public ICollection<User> GetAllWithProfile()
        {
            return UsersWithProfile.ToList();
        }

        public ICollection<User> GetAllWithGames()
        {
            return UsersWithGames.ToList();
        }

        public ICollection<User> GetAllWithAllData()
        {
            return UsersWithAllData.ToList();
        }
    }
}