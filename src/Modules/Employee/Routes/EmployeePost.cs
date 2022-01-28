using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

public class EmployeePost
{
  public static string template => "/employees";

  public static string[] methods => new string[] { HttpMethod.Post.ToString() };

  public static Delegate handle => Action;

  //Task é como se fosse uma promisse do javascript
  public static async Task<IResult> Action(EmployeeRequest employeeRequest, HttpContext http, UserManager<IdentityUser> userManager)
  {

    var newUser = new IdentityUser
    {
      UserName = employeeRequest.email,
      Email = employeeRequest.email,
    };

    //pega o id do usuario logado
    var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

    //cria de fato um empregado
    var result = await userManager.CreateAsync(newUser, employeeRequest.password);

    if (!result.Succeeded)
    {
      return Results.ValidationProblem(result.Errors.convertToProblemDetails());
    }


    //Cria uma lista de atributos do empregado
    //codigo do funcionario
    //nome do funcionario
    //por quem ele foi criado
    var userClaims = new List<Claim> {
      new Claim("EmployeeCode", employeeRequest.EmployeeCode),
      new Claim("Name", employeeRequest.name),
      new Claim("CreatedBy", userId)
    };

    //salva os atributos do usuario
    var claimResult =
    await userManager.AddClaimsAsync(newUser, userClaims);

    if (!claimResult.Succeeded)
    {
      return Results.ValidationProblem(result.Errors.convertToProblemDetails());
    }

    return Results.Created($"/employees/{newUser.Id}", newUser.Id);

  }
}