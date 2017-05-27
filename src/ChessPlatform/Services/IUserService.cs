using ChessPlatform.Entities;
using System;
using System.Collections.Generic;

namespace ChessPlatform.Services
{
    public interface IUserService
    {
        Tuple<bool, string> Create(User user, string password, string profileImagePath);
        void Update(User user);
        void Delete(User user);

        User GetUserById(string id);
        User GetUserByExternalId(int externalId);
        User GetUserByUsername(string username);
        User GetUserByEmail(string email);

        User GetUserWithProfileByExternalId(int externalId);
        User GetUserWithProfileByUsername(string username);

        User GetUserWithGamesByExternalId(int externalId);

        User GetUserWithAllDataByExternalId(int externalId);

        ICollection<User> GetAll();
        ICollection<User> GetAllWithProfileAndRank();
    }
}
