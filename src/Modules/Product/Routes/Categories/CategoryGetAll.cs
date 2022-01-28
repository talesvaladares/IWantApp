public class CategoryGetAll
{


  public static string template => "/categories";

  public static string[] methods => new string[] { HttpMethod.Get.ToString() };

  public static Delegate handle => Action;

  public static IResult Action(ApplicationDbContext context)
  {
    var categories = context.categories.ToList();
    var response = categories.Select(category => new CategoryResponse
    {
      name = category.name,
      active = category.active,
      id = category.id
    });

    return Results.Ok(response);

  }
}