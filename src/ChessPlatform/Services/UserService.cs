using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChessPlatform.Entities;
using ChessPlatform.Logging;
using ChessPlatform.Repositories;

namespace ChessPlatform.Services
{
    public class UserService : IUserService
    {
        #region Constructor and fields

        private readonly IUserRepository _userRepository;
        private readonly IApplicationLogger _logger;

        public UserService(IUserRepository userRepository, IApplicationLogger logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        #endregion

        #region Create and updates

        public Tuple<bool, string> Create(User user, string password, string profileImagePath)
        {
            try
            {
                lock (user)
                {
                    user.CreatedAt = DateTime.Now;
                    user.ExternalId = _userRepository.GetAll().Count() + 1;
                    user.Profile.Image = File.ReadAllBytes(profileImagePath);

                    var identityResult = _userRepository.Add(user, password);

                    if (identityResult.Result.Succeeded == false)
                    {
                        return new Tuple<bool, string>(false, identityResult.Result.Errors.FirstOrDefault().Description);
                    }

                    return new Tuple<bool, string>(true, "");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return new Tuple<bool, string>(false, "An unexpected error has occured!");
        }

        public void Update(User user)
        {
            try
            {
                _userRepository.Update(user);
                _userRepository.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }
        }

        public void Delete(User user)
        {
            try
            {
                _userRepository.Delete(user);
                _userRepository.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }
        }

        #endregion

        #region Getters

        #region Core

        //All data will be cached here.
        //At this time i don't know if it's a good ideea to cache large amount of data.

        #endregion

        public User GetUserById(string id)
        {
            try
            {
                return _userRepository.GetById(id);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return null;
        }

        public User GetUserByExternalId(int externalId)
        {
            try
            {
                return _userRepository.GetByExternalId(externalId);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return null;
        }

        public User GetUserByUsername(string username)
        {
            try
            {
                return _userRepository.GetByUsername(username);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return null;
        }

        public User GetUserByEmail(string email)
        {
            try
            {
                return _userRepository.GetByEmail(email);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return null;
        }


        public User GetUserWithProfileByExternalId(int externalId)
        {
            try
            {
                var user = _userRepository.GetWithProfileByExternalId(externalId);
                return user;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return null;
        }

        public User GetUserWithProfileByUsername(string username)
        {
            try
            {
                var user = _userRepository.GetWithProfileByUsername(username);
                return user;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return null;
        }


        public User GetUserWithGamesByExternalId(int externalId)
        {
            try
            {
                return _userRepository.GetWithGamesByExternalId(externalId);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return null;
        }


        public User GetUserWithAllDataByExternalId(int externalId)
        {
            try
            {
                IList<User> users = GetAllWithProfileAndRank().ToList();
                var user = users.Where(x => x.ExternalId == externalId).FirstOrDefault();

                user.Profile.Rank = users.IndexOf(user) + 1;

                return _userRepository.GetWithAllDataByExternalId(externalId);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return null;
        }


        public ICollection<User> GetAll()
        {
            try
            {
                return _userRepository.GetAll();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return null;
        }

        public ICollection<User> GetAllWithProfileAndRank()
        {
            try
            {
                IList<User> users = _userRepository.GetAllWithProfile().OrderByDescending(x => x.Profile.Points).ToList();

                foreach (var user in users)
                {
                    user.Profile.Rank = users.IndexOf(user) + 1;
                }

                return users;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception);
            }

            return null;
        }

        #endregion
    }
}