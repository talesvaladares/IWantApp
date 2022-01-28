using Dapper;
using Microsoft.Data.SqlClient;

public class QueryAllUserWithClaimNameService
{
  private readonly IConfiguration configuration;

  public QueryAllUserWithClaimNameService(IConfiguration configuration)
  {
    this.configuration = configuration;
  }

  public async Task<IEnumerable<EmployeeResponse>> execute(int page, int rows)
  {
    var db = new SqlConnection(configuration["Database:IWantDb"]);

    var query =
      @"select Email, ClaimValue as Name from AspNetUsers u inner join AspnetUserClaims c
      on u.id = c.UserId and claimType = 'Name'
      order by Name
      OFFSET (@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY
      ";

    return await db.QueryAsync<EmployeeResponse>(query, new { page, rows });
  }
}