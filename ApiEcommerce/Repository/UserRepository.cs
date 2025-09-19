using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;

namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public User? GetUser(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<User> GetUsers()
    {
        return _db.Users.OrderBy(u => u.Username).ToList();
    }

    public bool IsUniqueUser(string username)
    {
        return !_db.Users.Any(u => u.Username.ToLower().Trim() == username.ToLower().Trim());
    }

    public Task<UserLoginResponseDto?> Login(UserLoginDto userLoginDto)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> Register(CreateUserDto createUserDto)
    {
        var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        var user = new User
        {
            Name = createUserDto.Name,
            Username = createUserDto.Username ?? "No Username",
            Password = encryptedPassword,
            Role = "admin"
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(); // Guardar cambios de forma asíncrona
        return user;
    }
}
