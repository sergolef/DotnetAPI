using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using DotnetAPI.ModelsDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    IUserRepository _userRepository;
    IMapper _mapper;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;
    }

    [HttpGet("GetSignleUsers/{userId}")]
    public IActionResult GetSingleUser(int userId)
    {
        User? user = _userRepository.GetSingleUser(userId);

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

        _userRepository.AddEntity<User>(userDb);
        if(_userRepository.SaveChanges())
        {
           return Ok(); 
        }
        return BadRequest("Error of saving new user");
    }

    [HttpPut("UpdateUser")]
    public IActionResult EditUser(User userToUpd)
    {
        User? userDb;

        userDb = _userRepository.GetSingleUser(userToUpd.UserId);

        if(userDb != null)
        {
             _mapper.Map(userToUpd, userDb);
            if(_userRepository.SaveChanges())
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

        userDb = _userRepository.GetSingleUser(userId);

        if(userDb != null)
        {
            _userRepository.RemoveEntity<User>(userDb);
            if(_userRepository.SaveChanges())
            {
                return Ok();
            }
        }
        return BadRequest("Failed to delete user");
    }


    [HttpGet("GetUserSalary/{userId}")]
    public IActionResult GetUserSalary(int userId)
    {
    
        var userSalary = _userRepository.GetUserSalary(userId);

        if(userSalary == null)
        {
            return BadRequest("UserSalary is not found");
        }
        return Ok(userSalary);
    }


    [HttpGet("GetUserJobInfo/{userId}")]
    public IActionResult GetUserJobInfo(int userId)
    {
        IEnumerable<UserJobInfo> userJobInfo;

        userJobInfo =_userRepository.GetUserJobInfo(userId);

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

        _userRepository.AddEntity<UserSalary>(userDb);
        if(_userRepository.SaveChanges())
        {
           return Ok(); 
        }
        return BadRequest("Error of saving new user");
    }

    [HttpPost("CreateUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userSalaryDto)
    {
        UserJobInfo userDb = _mapper.Map<UserJobInfo>(userSalaryDto);

        _userRepository.AddEntity<UserJobInfo>(userDb);
        if(_userRepository.SaveChanges())
        {
           return Ok(); 
        }
        return BadRequest("Error of saving new user");
    }


    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {

        IEnumerable<UserSalary> salaries;

        salaries = _userRepository.GetUserSalary(userId);

        if(salaries != null)
        {
            foreach( UserSalary salary in salaries)
            {
                _userRepository.RemoveEntity<UserSalary>(salary);
                if(!_userRepository.SaveChanges())
                {
                    return BadRequest("Failed to delete user salary");
                }
            }
        }
        return Ok();
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {

        IEnumerable<UserJobInfo> jobs;

        jobs = _userRepository.GetUserJobInfo(userId);

        if(!jobs.IsNullOrEmpty())
        {
            foreach(UserJobInfo job in jobs)
            {
                _userRepository.RemoveEntity<UserJobInfo>(job);
                if(!_userRepository.SaveChanges())
                {
                    BadRequest("Failed to delete user job info");
                }
            }
            
        }
        return Ok();
    }
}
