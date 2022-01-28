using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class CategoryPost
{


  public static string template => "/categories";

  public static string[] methods => new string[] { HttpMethod.Post.ToString() };

  public static Delegate handle => Action;


  //Protejo a rota
  [Authorize(Policy = "EmployeePolicy")]
  public static async Task<IResult> Action(CategoryRequest categoryRequest, HttpContext http, ApplicationDbContext context)
  {

    // if (string.IsNullOrEmpty(categoryRequest.name))
    // {
    //   return Results.BadRequest("Name is required!");
    // }

    var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

    var category = new Category(categoryRequest.name, userId, userId);


    if (!category.IsValid)
    {
      return Results.BadRequest(category.Notifications);
    }

    await context.categories.AddAsync(category);
    await context.SaveChangesAsync();

    return Results.Created($"/categories/{category.id}", category.id);

  }
}