using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entityFramework;

        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }

        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null)
            {
                _entityFramework.Remove(entityToRemove);
            }
        }

        public IEnumerable<User> GetUsers()
        {

            IEnumerable<User> users = _entityFramework.Users.ToList<User>();
            return users;
        }

        public User GetSingleUser(int userId)
        {
            User? user;

            user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

            if(user == null){
                throw new Exception("User is not found");
            }
            return user;
        }

        public IEnumerable<UserSalary> GetUserSalary(int userId)
        {
            return _entityFramework.UserSalary.Where(u => u.UserId == userId).ToList<UserSalary>();
        }

        public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
        {
            return _entityFramework.UserJobInfo.Where(u => u.UserId == userId).ToList<UserJobInfo>();
        }
    }
}