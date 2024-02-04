using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controller
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
        }

        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlCheckUserExists = @"Select 
                [PasswordSalt], [PasswordHash] from TutorialAppSchema.Auth where Email = '"
                    + userForLogin.Email + "'";

                UserForLoginConfirmationDto userForConfirmation = _dapper
                    .LoadDataSingle<UserForLoginConfirmationDto>(sqlCheckUserExists);

            byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSolt);
            

            for(int index = 0; index < passwordHash.Length; index++)
            {
                if(passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    Console.WriteLine(passwordHash[index].ToString());
                    Console.WriteLine(userForConfirmation.PasswordHash[index]);
                    //return StatusCode(401, "Incorrect Password!");
                }
            }
            return Ok();
        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value 
                        + Convert.ToBase64String(passwordSalt);

            byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
                return passwordHash; 
        }


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

                    byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

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
                        return Ok();
                    }
                    throw new Exception("Failed to register user.");
                    
                } 
                throw new Exception("Yser with this email already exists");
                
            }
            throw new Exception("Passwords does not mutch");
            
        }
    }
}