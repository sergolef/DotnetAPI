using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Helpers
{
    public class ReusableSql
    {

        private readonly DataContextDapper _dapper;

        public ReusableSql(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public bool UpsertUser(UserComplete user)
        {
            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = @inputFirstName,
            @LastName = @inputLastName,
            @Email = @inputEmail, 
            @Gender = @inputGender, 
            @JobTitle = @inputJobTitle, 
            @Department = @inputDepartment, 
            @Salary = @inputSalary,
            @Active = @inputActive,
            @UserId = @inputUserId";

            DynamicParameters dinParams = new DynamicParameters();
            dinParams.Add("@inputFirstName", user.FirstName, DbType.String);
            dinParams.Add("@inputLastName", user.LastName, DbType.String);
            dinParams.Add("@inputEmail", user.Email, DbType.String);
            dinParams.Add("@inputGender", user.Gender, DbType.String);
            dinParams.Add("@inputJobTitle", user.JobTitle, DbType.String);
            dinParams.Add("@inputDepartment", user.Department, DbType.String);
            dinParams.Add("@inputSalary", user.Salary, DbType.Decimal);
            dinParams.Add("@inputActive", user.Active, DbType.Boolean);
            dinParams.Add("@inputUserId", user.UserId, DbType.Int32);

            Console.WriteLine(sql);
            return _dapper.ExecuteSqlWithParams(sql, dinParams);
        }
    }
}