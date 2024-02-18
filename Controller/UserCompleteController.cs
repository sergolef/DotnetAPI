using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetUsers/{usersId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int usersId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get ";
        string parameters = "";
        DynamicParameters dinParams = new DynamicParameters();

        if (usersId != 0)
        {
            parameters += ", @UserId=@UserParam";
            dinParams.Add("@UserParam", usersId, DbType.Int32);
        }
        if (isActive)
        {
            parameters += ", @Active=@ActiveParam";
            dinParams.Add("@ActiveParam", isActive, DbType.Boolean);
        }
        sql += parameters[1..];

        IEnumerable<UserComplete> users;

        users = _dapper.LoadDataWithParams<UserComplete>(sql, dinParams);
        return users;
    }


    [HttpPut("UpdateUser")]
    public IActionResult EditUser(UserComplete user)
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
        if (_dapper.ExecuteSqlWithParams(sql, dinParams))
        {
            return Ok();
        }
        return BadRequest("Failed updating user");

    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @" EXEC TutorialAppSchema.SPUser_Delete
         @UserId = @UserIdParam'" + userId.ToString() + "'";
        DynamicParameters dinParams = new DynamicParameters();
        dinParams.Add("@UserIdParam", userId, DbType.Int32);
        
        Console.WriteLine(sql);
        if (_dapper.ExecuteSqlWithParams(sql, dinParams))
        {
            return Ok();
        }
        return BadRequest("Failed to delete user");
    }

}
