using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    private string? secretKey;

    public UserRepository(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
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

    public async Task<UserLoginResponseDto?> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Username))
        {
            return new UserLoginResponseDto
            {
                Token = "",
                User = null,
                Message = "El Username es requerido"
            };
        }

        var user = await _db.Users.FirstOrDefaultAsync<User>(u => u.Username.ToLower().Trim() == userLoginDto.Username.ToLower().Trim());

        if (user == null)
        {
            return new UserLoginResponseDto
            {
                Token = "",
                User = null,
                Message = "Username no encontrado"
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password)) // Verificar la contraseña encriptada
        {
            return new UserLoginResponseDto
            {
                Token = "",
                User = null,
                Message = "Credenciales son incorrectas"
            };
        }

        // JWT Generación
        var handlerToken = new JwtSecurityTokenHandler();

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("SecretKey no está configurada.");
        }

        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] // Arreglo de Claims
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty) // Usar cadena vacía si Role es null
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Algoritmo de encriptación
        };

        var token = handlerToken.CreateToken(tokenDescriptor);

        return new UserLoginResponseDto
        {
            Token = handlerToken.WriteToken(token),
            User = new UserRegisterDto
            {
                Name = user.Name,
                Username = user.Username,
                Role = user.Role,
                Password = user.Password ?? string.Empty
            },
            Message = "Usuario logueado correctamente"
        };
    }

    public async Task<User?> Register(CreateUserDto createUserDto)
    {
        var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        var user = new User
        {
            Name = createUserDto.Name,
            Username = createUserDto.Username ?? "No Username",
            Password = encryptedPassword,
            Role = createUserDto.Role
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(); // Guardar cambios de forma asíncrona
        return user;
    }
}
