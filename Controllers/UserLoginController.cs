using Identity.Domain.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.ModelViews;
using Identity.Domain.Services;
using Identity.Infrastructure.Repositories.Interfaces;
using Identity.Utils;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Identity.Controllers;

[ApiController]
[Route("api/users")]
public class UserLoginController : ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly ITokenJwt _tokenJwt;
    private readonly ICrypto _crypto;

    public UserLoginController(IRepository<User> userRepository, ITokenJwt tokenJwt, ICrypto crypto)
    {
        _crypto = crypto;
        _tokenJwt = tokenJwt;
        _userRepository = userRepository;
    }

    [HttpPost("/insert")]
    public async Task<IActionResult> Insert(UserDto userDto)
    {
        var users = await _userRepository.FindAsync(a => a.Email == userDto.Email);
        if(users.Count() == 0)
        {
            var salt = _crypto.GetSalt();
            var user = new User(){
                Name = userDto.Name,
                Email = userDto.Email,
                Role = Converter.GetRole(userDto.Role),
                Password = _crypto.Encrypt(userDto.Password, salt),
                Salt = salt
            };

            await _userRepository.AddAsync(user);
        }

        return Ok(new HttpReturn{ Message = "Usuário criado com sucesso" });
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var userList = await _userRepository.FindAsync(a => a.Email == request.Email);
        if (userList.Count() == 0)
            return NotFound(new HttpReturn{ Message = "Usuário não cadastrado" });

        var user = userList.First();
        var pass = _crypto.Encrypt(request.Password, user.Salt);

        if(user.Password != pass)
            return BadRequest(new HttpReturn{ Message = "Credenciais inválidas." });

        var simpleUser = SimpleUser.Build(user);
        return Ok(new LoggedUser
        { 
            User = simpleUser,
            Token = new UserToken(_tokenJwt).BuildToken(simpleUser)
        });
    }

    [HttpPost("/refresh-token")]
    public IActionResult RefreshToken()
    {
        var user = GetUserFromToken();
        if(user == null) return Forbid();

        return Ok(new LoggedUser
        { 
            User = user,
            Token = new UserToken(_tokenJwt).BuildToken(user)
        });
    }

    [HttpHead("/valid-token")]
    public IActionResult ValidToker()
    {
        return GetUserFromToken() != null ? Ok() : Forbid();
    }

    [HttpHead("/valid-role-token")]
    public IActionResult ValidRoleToken()
    {
        return ValidRoleFromToken() ? Ok() : Forbid();
    }

    private SimpleUser GetUserFromToken()
    {
        string authorizationHeader = Request.Headers["Authorization"];
        if (!string.IsNullOrWhiteSpace(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
        {
            string token = authorizationHeader.Substring("Bearer ".Length);
            var user = new UserToken(_tokenJwt).TokenToUser(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            return user;
        }
        return null;
    }

    private bool ValidRoleFromToken()
    {
        string authorizationHeader = Request.Headers["Authorization"];
        string role = Request.Headers["Role"].ToString().ToLower();
        if (!string.IsNullOrWhiteSpace(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
        {
            string token = authorizationHeader.Substring("Bearer ".Length);
            var user = new UserToken(_tokenJwt).TokenToUser(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var roles = jwtToken?.Claims.Where(c => c.Type == "role").Select(c => c.Value).Where(r => r == role).FirstOrDefault();
            return roles.Any();
        }
        return false;
    }
    
}
