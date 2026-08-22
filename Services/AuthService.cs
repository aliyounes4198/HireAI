using HireAI.API.Data;
using HireAI.API.DTOs;
using HireAI.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HireAI.API.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }



        public async Task<User?> Register(RegisterDto dto)
        {
            // Check if email already exists

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);


            if (existingUser != null)
            {
                return null;
            }


            // Hash password

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);



            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.Role,
                CompanyId = dto.CompanyId
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();


            return user;
        }




        public async Task<string?> Login(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);



            if (user == null)
            {
                return null;
            }



            bool passwordValid =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    user.PasswordHash
                );



            if (!passwordValid)
            {
                return null;
            }



            return GenerateToken(user);
        }





        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    user.Role
                ),

                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                )
            };



            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );



            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );



            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials
            );



            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}