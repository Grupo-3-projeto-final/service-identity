using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Identity.Domain.ModelViews;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Domain.Services;

public class UserToken
{
    public UserToken(ITokenJwt tokenJwt)
    {
        _time = TimeSpan.FromDays(Convert.ToInt16(Environment.GetEnvironmentVariable("IDENTITY_TIME_JWT")));
        _secret = Environment.GetEnvironmentVariable("IDENTITY_SECRET_JWT");
        _tokenJwt = tokenJwt;
    }

    private TimeSpan _time;
    private string _secret;
    private ITokenJwt _tokenJwt;

    public string BuildToken(SimpleUser User)
    {
        var claims = new List<Claim>
        {
            new Claim("value", JsonSerializer.Serialize(User)),
            new Claim(ClaimTypes.Name, User.Name),
            new Claim(ClaimTypes.Email, User.Email),
            new Claim("role", User.RoleName)
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            Expires = DateTime.UtcNow.Add(_time),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public SimpleUser TokenToUser(string token)
    {
        try
        {
            var jsonUser = _tokenJwt.Decrypt(token, _secret);
            return JsonSerializer.Deserialize<SimpleUser>(jsonUser);
        }
        catch
        {
            return null;
        }
    }
}