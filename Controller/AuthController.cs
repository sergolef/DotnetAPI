using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
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

        public AuthController(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(_config);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlCheckUserExists = @"Select 
                [PasswordSalt], [PasswordHash] from TutorialAppSchema.Auth where Email = '"
                    + userForLogin.Email + "'";

                UserForLoginConfirmationDto userForConfirmation = _dapper
                    .LoadDataSingle<UserForLoginConfirmationDto>(sqlCheckUserExists);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);
            
            if(userForConfirmation == null)
            {
                return StatusCode(401, "Incorrect Password!!");
            }

            for(int index = 0; index < passwordHash.Length; index++)
            {
                if(passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect Password!");
                }
            }
            string userIdSql = @"Select UserId From TutorialAppSchema.Users Where Email = '"+
                userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);
            
            return Ok( new Dictionary<string, string>{
                {
                    "token", _authHelper.CreateToken(userId)
                }
            });
        }

        

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if(userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "Select Email from TutorialAppSchema.Auth where email = '"
                    + userForRegistration.Email + "'";

                IEnumerable<string> existingUSers = _dapper.LoadData<string>(sqlCheckUserExists);
                if(existingUSers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);
                    

                    string sqlAddAuth = @"Insert into TutorialAppSchema.Auth([Email], 
                    [PasswordHash], [PasswordSalt]) Values('" + userForRegistration.Email 
                    + "', @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;


                    sqlParameters.Add(passwordHashParameter);
                    sqlParameters.Add(passwordSaltParameter);
                     
                    if(_dapper.ExecuteSqlWithParams(sqlAddAuth, sqlParameters))
                    {
                        string sqlUser  = @"Insert TutorialAppSchema.Users 
                            ([FirstName], [LastName], [Email], [Gender], [Active])
                            VALUES ('"+ userForRegistration.FirstName +
                            "', '" + userForRegistration.LastName +
                            "','" + userForRegistration.Email +
                            "','" + userForRegistration.Gender +
                            "', 1)";
       
                        if(_dapper.ExecuteSql(sqlUser))
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


        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            
            string userIdSql = @"Select UserId From TutorialAppSchema.Users Where UserId = '"+
               User.FindFirst("userId")?.Value  + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);  

            return _authHelper.CreateToken(userId);
        }
    }
}