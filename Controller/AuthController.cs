using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;


namespace DotnetAPI.Controller
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;

        private readonly AuthHelper _authHelper;

        private readonly IMapper _mapper;
        
        private readonly ReusableSql _reusableSql;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(_config);
            _mapper = new Mapper(new MapperConfiguration( config => {
                config.CreateMap<UserForRegistrationDto, UserComplete>();
            }));
            _reusableSql = new ReusableSql(config);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlCheckUserExists = @"EXEC TutorialAppSchema.spLoginConfirmation_Get @Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();

            // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            // emailParameter.Value = userForLogin.Email;
            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

            UserForLoginConfirmationDto userForConfirmation = 
                _dapper.LoadDataSingleWithParams<UserForLoginConfirmationDto>(sqlCheckUserExists, sqlParameters);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            if (userForConfirmation == null)
            {
                return StatusCode(401, "Incorrect Password!!");
            }

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect Password!");
                }
            }
            string userIdSql = @"Select UserId From TutorialAppSchema.Users Where Email = '" +
                userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>{
                {
                    "token", _authHelper.CreateToken(userId)
                }
            });
        }



        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = @"Select Email from TutorialAppSchema.Auth where email = '"
                    + userForRegistration.Email + "';";
                    //Console.WriteLine(sqlCheckUserExists);return Ok();

                IEnumerable<string> existingUSers = _dapper.LoadData<string>(sqlCheckUserExists);
   
                if (existingUSers.Count() == 0)
                {
                   
                    UserForLoginDto userForLogin = new UserForLoginDto() {
                        Password = userForRegistration.Password,
                        Email = userForRegistration.Email
                    };

                    

                    if (_authHelper.SetPassword(userForLogin))
                    {
                        UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
                        userComplete.Active = true;

                        if (_reusableSql.UpsertUser(userComplete))
                        {
                            return Ok();
                        }
                        throw new Exception("Failer of creation new user");
                    }
                    throw new Exception("Failed to register user.");

                }
                throw new Exception("Yser with this email already exists");

            }
            throw new Exception("Passwords does not mutch");

        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDto userForLogin)
        {
            if (_authHelper.SetPassword(userForLogin))
            {
                return Ok();
            }
            throw new Exception("Reset poassword is failed");
        }


        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {

            string userIdSql = @"Select UserId From TutorialAppSchema.Users Where UserId = '" +
               User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }
    }
}