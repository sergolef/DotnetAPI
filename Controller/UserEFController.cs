using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using DotnetAPI.ModelsDtos;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    DataContextEF _entityFramework;
    IMapper _mapper;
    public UserEFController(IConfiguration config)
    {
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserDto, User>();
            cfg.CreateMap<User, User>();
            cfg.CreateMap<UserSalary, UserSalary>();
            cfg.CreateMap<UserJobInfo, UserJobInfo>();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
       
        IEnumerable<User> users = _entityFramework.Users.ToList<User>();
        return users;
    }

    [HttpGet("GetSignleUsers/{userId}")]
    public IActionResult GetSingleUser(int userId)
    {
        User? user;

        user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

        if(user == null)
        {
            return BadRequest("User is not found");
        }
        return Ok(user);
    }

    [HttpPost("CreateUser")]
    public IActionResult AddUser(UserDto userDto)
    {
        User userDb = _mapper.Map<User>(userDto);

        _entityFramework.Add(userDb);
        if(_entityFramework.SaveChanges() > 0)
        {
           return Ok(); 
        }
        return BadRequest("Error of saving new user");
    }

    [HttpPut("UpdateUser")]
    public IActionResult EditUser(User userToUpd)
    {
        User? userDb;

        userDb = _entityFramework.Users.Where(u => u.UserId == userToUpd.UserId)
            .FirstOrDefault<User>();

        if(userDb != null)
        {
             _mapper.Map(userToUpd, userDb);
            if(_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
        }
       
        return BadRequest("Failed updating user");
        
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {

        User? userDb;

        userDb = _entityFramework.Users.Where(u => u.UserId == userId)
            .FirstOrDefault<User>();

        if(userDb != null)
        {
            _entityFramework.Users.Remove(userDb);
            if(_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
        }
        return BadRequest("Failed to delete user");
    }


    [HttpGet("GetUserSalary/{userId}")]
    public IActionResult GetUserSalary(int userId)
    {
        

        var userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).ToList<UserSalary>();

        if(userSalary == null)
        {
            return BadRequest("UserSalary is not found");
        }
        return Ok(userSalary);
    }


    [HttpGet("GetUserJobInfo/{userId}")]
    public IActionResult GetUserJobInfo(int userId)
    {
        UserJobInfo? userJobInfo;

        userJobInfo = _entityFramework.UserJobInfo.Where(u => u.UserId == userId).FirstOrDefault<UserJobInfo>();

        if(userJobInfo == null)
        {
            return BadRequest("UserJobInfo is not found");
        }
        return Ok(userJobInfo);
    }


    [HttpPost("CreateUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        UserSalary userDb = _mapper.Map<UserSalary>(userSalary);

        _entityFramework.Add(userDb);
        if(_entityFramework.SaveChanges() > 0)
        {
           return Ok(); 
        }
        return BadRequest("Error of saving new user");
    }

    [HttpPost("CreateUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userSalaryDto)
    {
        UserJobInfo userDb = _mapper.Map<UserJobInfo>(userSalaryDto);

        _entityFramework.Add(userDb);
        if(_entityFramework.SaveChanges() > 0)
        {
           return Ok(); 
        }
        return BadRequest("Error of saving new user");
    }


    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {

        UserSalary? userDb;

        userDb = _entityFramework.UserSalary.Where(u => u.UserId == userId)
            .FirstOrDefault<UserSalary>();

        if(userDb != null)
        {
            _entityFramework.UserSalary.Remove(userDb);
            if(_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
        }
        return BadRequest("Failed to delete user salary");
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {

        UserJobInfo? userDb;

        userDb = _entityFramework.UserJobInfo.Where(u => u.UserId == userId)
            .FirstOrDefault<UserJobInfo>();

        if(userDb != null)
        {
            _entityFramework.UserJobInfo.Remove(userDb);
            if(_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
        }
        return BadRequest("Failed to delete user job info");
    }
}
