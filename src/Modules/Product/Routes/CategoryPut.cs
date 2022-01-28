using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class CategoryPut
{


  public static string template => "/categories/{id:guid}";

  public static string[] methods => new string[] { HttpMethod.Put.ToString() };

  public static Delegate handle => Action;

  //Protejo a rota
  [Authorize(Policy = "EmployeePolicy")]
  public static async Task<IResult> Action([FromRoute] Guid id, HttpContext http, CategoryRequest categoryRequest, ApplicationDbContext context)
  {

    var category = context.categories.Where(category => category.id == id).FirstOrDefault();
    var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

    if (category == null)
    {

      return Results.ValidationProblem(category.Notifications.convertToProblemDetails());
    }

    category.editInfo(category.name, category.active, userId);

    if (!category.IsValid)
    {
      return Results.ValidationProblem(category.Notifications.convertToProblemDetails());
    }

    await context.SaveChangesAsync();

    return Results.Ok();

  }
}