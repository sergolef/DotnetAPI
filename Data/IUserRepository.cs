using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        
        public void AddEntity<T>(T entityToAdd);

        public void RemoveEntity<T>(T entityToRemove);

        public IEnumerable<User> GetUsers();

        public User GetSingleUser(int userId);

        public IEnumerable<UserSalary> GetUserSalary(int userId);
        public IEnumerable<UserJobInfo> GetUserJobInfo(int userId);
        
    }
}