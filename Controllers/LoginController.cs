using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using MiddlewareFilterDI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using MiddlewareFilterDI.Models;

namespace MiddlewareFilterDI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _db;

        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger, MyDbContext db, IConfiguration configuration)
        {
            _logger = logger;
            _db = db;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<IActionResult> login(LoginCredential lc)
        {
            string jsonResp = "";

            try
            {
                _logger.LogInformation("Login attempt for LoginId: {LoginId}", lc.LoginId);
                //int phone = int.Parse(lc.phoneno);
                var user = await _db.LoginCredential
                    .FirstOrDefaultAsync(u =>
                    u.LoginId == lc.LoginId &&
                    u.Password == lc.Password
                    );

                if (user != null)
                {
                    var token = GenerateJwtToken(user.LoginId, user.type);
                    _logger.LogInformation("Login successful for Email: {Email}", lc.LoginId);

                    jsonResp = JsonConvert.SerializeObject(new
                    {

                        Status = "0",
                        Message = "Login Successfulssssss",
                        Token = token,
                        Role = user.type,
                        ExpiresIn = _configuration["Jwt:DurationInMinutes"],
                        Error = Array.Empty<object>()
                    });
                }
                else
                {
                    _logger.LogWarning("Enter Valid loginId and Password for LoginId: {LoginId} & Password:{Password}", lc.LoginId,lc.Password);
                    jsonResp = JsonConvert.SerializeObject(new
                    {
                        Status = "1",
                        Message = "Enter Valid loginId and Password",
                        Token = "NA",
                        ExpiresIn = "NA",
                        Error = Array.Empty<object>()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during login for LoginId: {LoginId}", lc.LoginId);
                jsonResp = JsonConvert.SerializeObject(new
                {
                    Status = "-1",
                    Message = Array.Empty<object>(),
                    Token = "NA",
                    ExpiresIn = "NA",
                    Error = ex.Message
                });
            }
            Response.StatusCode = StatusCodes.Status200OK;
            return Content(jsonResp, "application/json");
        }
        private string GenerateJwtToken(string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class loginClass
        {
            public string Email { get; set; }
            public string phoneno { get; set; }
        }
    }
}
