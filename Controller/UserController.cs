using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = "SELECT [UserId], [FirstName], [LastName], [Email], [Gender], [Active] from TutorialAppSchema.Users";
        IEnumerable<User> users;

        users = _dapper.LoadData<User>(sql);
        return users;
    }

    [HttpGet("GetSignleUsers/{userId}")]
    public IActionResult GetSingleUser(int userId)
    {
        string sql = @"SELECT [UserId], [FirstName], [LastName], [Email], [Gender], [Active] 
            from TutorialAppSchema.Users
            Where UserId ='" + userId + "'";
        User user;

        user = _dapper.LoadDataSingle<User>(sql);

        return Ok(user);
    }

    [HttpPost("CreateUser")]
    public IActionResult AddUser(UserDto userDto)
    {
        string sql  = @"Insert TutorialAppSchema.Users 
            ([FirstName], [LastName], [Email], [Gender], [Active])
                VALUES ('"+ userDto.FirstName +
                "', '" + userDto.LastName +
                "','" + userDto.Email +
                "','" + userDto.Gender +
                "','" + userDto.Active + "')";
        Console.WriteLine(sql);
        if(!_dapper.ExecuteSql(sql))
        {
            return BadRequest("Error of creation new user");
        }
        return Ok();
    }

    [HttpPut("UpdateUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"Update TutorialAppSchema.Users
        SET [FirstName] = '" + user.FirstName +
        "',[LastName] = '" + user.LastName +
        "',[Email] = '" + user.Email +
        "',[Gender] = '" + user.Gender +
        "',[Active] = '" + user.Active + "' Where UserId = "+user.UserId;
        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        return BadRequest("Failed updating user");
        
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.Users
         Where UserId = "+userId.ToString();
        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        return BadRequest("Failed to delete user");
    }
}
