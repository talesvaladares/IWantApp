using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;

public static class ProblemDetailsExtension
{
  public static Dictionary<string, string[]> convertToProblemDetails(this IReadOnlyCollection<Notification> notifications)
  {
    return notifications.GroupBy(group => group.Key)
        .ToDictionary(group => group.Key, group => group
        .Select(x => x.Message).ToArray());
  }

  public static Dictionary<string, string[]> convertToProblemDetails(this IEnumerable<IdentityError> error)
  {
    var dictionary = new Dictionary<string, string[]>();
    dictionary.Add("Error", error.Select(e => e.Description).ToArray());

    return dictionary;
  }
}