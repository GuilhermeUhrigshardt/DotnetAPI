using AutoMapper;
using DotnetAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContextEF _entityFramework;

    public UserRepository(IConfiguration configuration)
    {
        _entityFramework = new DataContextEF(configuration);
    }

    public bool SaveChanges()
    {
        return _entityFramework.SaveChanges() > 0;
    }

    public void AddEntity<T>(T entityToAdd)
    {
        _entityFramework.Add(entityToAdd!);
    }

    public void RemoveEntity<T>(T entityToRemove)
    {
        _entityFramework.Remove(entityToRemove!);
    }

    public IEnumerable<User> GetUsers()
    {
        return _entityFramework.Users.ToList();
    }

    public User GetSingleUser(int userId)
    {
        User user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault()!;
        if (user != null)
        {
            return user;
        }

        throw new Exception("--> User not found");
    }

    public UserSalary GetSingleUserSalary(int userId)
    {
        UserSalary userSalary = _entityFramework.UserSalaries.Where(us => us.UserId == userId).FirstOrDefault()!;
        if (userSalary != null)
        {
            return userSalary;
        }

        throw new Exception("--> User Salary not found");
    }

    public UserJobInfo GetSingleUserJobInfo(int userId)
    {
        UserJobInfo userJobInfo = _entityFramework.UserJobInfos.Where(uji => uji.UserId == userId).FirstOrDefault()!;
        if (userJobInfo != null)
        {
            return userJobInfo;
        }

        throw new Exception("--> User Job Info not found");
    }
}