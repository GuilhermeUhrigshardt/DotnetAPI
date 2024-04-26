using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.UserController;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase {
    private readonly DataContextDapper _dapper;

    public UserController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
    }

    [HttpGet("TestConnection")]
    public void TestConnection()
    {
        Console.WriteLine(_dapper.LoadDataSingle<DateTime>("SELECT GETDATE()"));
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        return _dapper.LoadDataSingle<User>($"SELECT * FROM TutorialAppSchema.Users WHERE UserID = {userId};");
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        return _dapper.LoadData<User>("SELECT * FROM TutorialAppSchema.Users;");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @$"
        UPDATE TutorialAppSchema.Users
        SET [FirstName] = '{user.FirstName}',
        [LastName] = '{user.LastName}',
        [Email] = '{user.Email}',
        [Gender] = '{user.Gender}',
        [Active] = {(user.Active ? 1 : 0)}
        WHERE UserId = {user.UserId}";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserAddDTO user)
    {
        string sql = @$"
        INSERT INTO TutorialAppSchema.Users
        (
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active]
        )
        VALUES (
            '{user.FirstName}',
            '{user.LastName}',
            '{user.Email}',
            '{user.Gender}',
            {(user.Active ? 1 : 0)}
        );";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = $"DELETE FROM TutorialAppSchema.Users WHERE UserId = {userId.ToString()};";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }
}