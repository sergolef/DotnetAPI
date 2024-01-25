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

    [HttpGet("UserJobInfo/{userId}")]
    public IActionResult GetUserJobInfo(int userId)
    {
         string sql = @"SELECT [UserId],[JobTitle],[Department] 
            from TutorialAppSchema.UserJobInfo
            Where UserId ='" + userId + "'";

        UserJobInfo jobInfo = _dapper.LoadDataSingle<UserJobInfo>(sql);

        if(jobInfo != null)
        {
            return Ok(jobInfo);
        }
        return BadRequest("Job info is not found");
    }

    [HttpGet("UserSalary/{userId}")]
    public IActionResult GetUserSalary(int userId)
    {
         string sql = @"SELECT [UserId], [Salary]
            from TutorialAppSchema.UserSalary
            Where UserId ='" + userId + "'";

        UserSalary salary = _dapper.LoadDataSingle<UserSalary>(sql);

        if(salary != null)
        {
            return Ok(salary);
        }
        return BadRequest("User salary is not found");
    }


    [HttpPost("UserSalary")]
    public IActionResult PostUserSalary(UserSalary salary)
    {

        string sql  = @"Insert TutorialAppSchema.UserSalary 
            ([UserId], [Salary])
                VALUES ('"+ salary.UserId +
                "', '" + salary.Salary + "')";
        Console.WriteLine(sql);
        if(!_dapper.ExecuteSql(sql))
        {
            return BadRequest("Error of creation new user salary");
        }
        return Ok();

    }

    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfo(UserJobInfo jobInfo)
    {

        string sql  = @"Insert TutorialAppSchema.UserJobInfo 
            ([UserId], [JobTitle], [Department])
                VALUES ('"+ jobInfo.UserId +
                "', '" + jobInfo.JobTitle +
                "', '" + jobInfo.Department + "')";
        Console.WriteLine(sql);
        if(!_dapper.ExecuteSql(sql))
        {
            return BadRequest("Error of creation new user job info");
        }
        return Ok();

    }


    [HttpPut("UserJobInfo")]
    public IActionResult EditUserSalary(UserJobInfo jobInfo)
    {
        string sql = @"Update TutorialAppSchema.UserJobInfo
        SET [JobTitle] = '" + jobInfo.JobTitle +
        "',[Department] = '" + jobInfo.Department +
        "' Where UserId = "+jobInfo.UserId;

        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        return BadRequest("Failed updating user job info");

    }

    [HttpPut("UserSalary")]
    public IActionResult EditUserSalary(UserSalary salary)
    {
        string sql = @"Update TutorialAppSchema.UserSalary
        SET [Salary] = '" + salary.Salary +
        "' Where UserId = "+salary.UserId;

        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        return BadRequest("Failed updating user salary info");

    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.UserSalary
         Where UserId = "+userId.ToString();
        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        return BadRequest("Failed to delete user salary");
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJob(int userId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.UserJobInfo
         Where UserId = "+userId.ToString();
        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        return BadRequest("Failed to delete user job info");
    }


}
