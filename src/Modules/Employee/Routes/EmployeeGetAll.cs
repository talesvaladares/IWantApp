
using Microsoft.AspNetCore.Authorization;

public class EmployeeGetAll
{


  public static string template => "/employees";

  public static string[] methods => new string[] { HttpMethod.Get.ToString() };

  public static Delegate handle => Action;

  [Authorize(Policy = "EmployeePolicy")]
  public static async Task<IResult> Action(int? page, int? rows, QueryAllUserWithClaimNameService query)
  {

    // //pego todos os usuarios
    // var users = userManager.Users.Skip((page - 1) * rows).Take(rows).ToList();


    // //crio uma lista vazia
    // var employees = new List<EmployeeResponse>();


    // foreach (var item in users)
    // {
    //   var claims = userManager.GetClaimsAsync(item).Result;
    //   var userName =
    //   claims.FirstOrDefault(c => c.Type == "Name").Value != null ? claims.FirstOrDefault(c => c.Type == "Name").Value : string.Empty;
    //   employees.Add(new EmployeeResponse(item.Email, userName));
    // }
    var result = await query.execute(page.Value, rows.Value);

    return Results.Ok(result);

  }
}