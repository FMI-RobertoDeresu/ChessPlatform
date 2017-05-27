using System.Collections.Generic;
using System.Threading.Tasks;
using ChessPlatform.Entities;
using Microsoft.AspNetCore.Identity;

namespace ChessPlatform.Repositories
{
    public interface IUserRepository : IRepository
    {
        Task<IdentityResult> Add(User newUser, string password);
        void Update(User user);
        void Delete(User user);

        User GetById(string id);
        User GetByExternalId(int externalId);
        User GetByUsername(string username);
        User GetByEmail(string email);

        User GetWithProfileById(string id);
        User GetWithProfileByExternalId(int externalId);
        User GetWithProfileByUsername(string username);
        User GetWithProfileByEmail(string email);

        User GetWithGamesById(string id);
        User GetWithGamesByExternalId(int externalId);
        User GetWithGamesByUsername(string username);
        User GetWithGamesByEmail(string email);

        User GetWithAllDataById(string id);
        User GetWithAllDataByExternalId(int externalId);
        User GetWithAllDataByUsername(string username);
        User GetWithAllDataByEmail(string email);

        ICollection<User> GetAll();
        ICollection<User> GetAllWithProfile();
        ICollection<User> GetAllWithGames();
        ICollection<User> GetAllWithAllData();
    }
}