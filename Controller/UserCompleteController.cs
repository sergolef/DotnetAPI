using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    private readonly ReusableSql _reusableSql;
    public UserCompleteController(IConfiguration config)
    {
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
       _reusableSql = new ReusableSql(config);
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
        if (_reusableSql.UpsertUser(user))
        {
            return Ok();
        }
        return BadRequest("Failed updating user");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @" EXEC TutorialAppSchema.SPUser_Delete
         @UserId = @UserIdParam";
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
