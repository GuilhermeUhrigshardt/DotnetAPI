using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers;

public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
        _configuration = configuration;
    }

    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password.Equals(userForRegistration.PasswordConfirm))
        {
            string sqlCheckUserExists = $"SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '{userForRegistration.Email}';";

            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

            if (existingUsers.Count() == 0)
            {
                byte[] passwordSalt = new byte[120 / 8];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(passwordSalt);
                }

                byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

                string sqlAddAuth = $@"
                    INSERT INTO TutorialAppSchema.Auth ([Email],
                    [PasswordHash],
                    [PasswordSalt]) VALUES ('{userForRegistration.Email}', @PasswordHash, @PasswordSalt);";

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                passwordSaltParameter.Value = passwordSalt;
                SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                passwordHashParameter.Value = passwordHash;

                sqlParameters.Add(passwordSaltParameter);
                sqlParameters.Add(passwordHashParameter);

                if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                {
                    return Ok();
                }
                throw new Exception("--> Failed to register User!");
            }
            throw new Exception("--> User with this email already exists!");
        }
        throw new Exception("--> Passwords do not match!");
    }
        

    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt = $@"SELECT
            [PasswordHash],
            [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '{userForLogin.Email}';";

        UserForLoginConfirmationDto userForLoginConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

        byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

        for (int i = 0; i < passwordHash.Length; i++)
        {
            if (passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
                return StatusCode(401, "Incorrect password!");
        }

        return Ok();
    }

    private byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusString = _configuration.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1000000,
            numBytesRequested: 256 / 8
        );
    }
}