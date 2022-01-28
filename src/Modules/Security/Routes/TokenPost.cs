using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

public class TokenPost
{


  public static string template => "/token";

  public static string[] methods => new string[] { HttpMethod.Post.ToString() };

  public static Delegate handle => Action;

  //Permite que a rota seja usada sem autenticação
  [AllowAnonymous]
  public static IResult Action(
    LoginRequest loginRequest,
    IConfiguration configuration,
    UserManager<IdentityUser> userManager,
    ILogger<TokenPost> log
  )
  {

    var user = userManager.FindByEmailAsync(loginRequest.Email).Result;

    if (user == null)
    {
      return Results.BadRequest();
    }

    if (!userManager.CheckPasswordAsync(user, loginRequest.Password).Result)
    {
      return Results.BadRequest();
    }

    var claims = userManager.GetClaimsAsync(user).Result;
    var subject = new ClaimsIdentity(new Claim[]{
        new Claim(ClaimTypes.Email, loginRequest.Email),
        new Claim(ClaimTypes.NameIdentifier, user.Id)
      });
    subject.AddClaims(claims);

    var key = Encoding.UTF8.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = subject,
      SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
      ),
      Audience = configuration["JwtBearerTokenSettings:Audience"],
      Issuer = configuration["JwtBearerTokenSettings:Issuer"],
      Expires = DateTime.UtcNow.AddSeconds(30)
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return Results.Ok(new
    {
      token = tokenHandler.WriteToken(token)
    });

  }
}