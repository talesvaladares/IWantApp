using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);


//configuração de para salvar os logs no banco de dados
//configuração feita para sqlServer
builder.WebHost.UseSerilog((context, configuration) =>
{
  configuration
  .WriteTo.Console()
  .WriteTo.MSSqlServer(
    context.Configuration["Database:IWantDb"],
    sinkOptions: new MSSqlServerSinkOptions()
    {
      AutoCreateSqlTable = true,
      TableName = "LogAPI"
    }
  );
});


//inicialização do banco de dados
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:IWantDb"]);
//configuração de autenticação de usuario com a aplicação e banco
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireDigit = false;
  options.Password.RequireUppercase = false;
  options.Password.RequiredLength = 3;
  options.Password.RequireLowercase = false;
})
  .AddEntityFrameworkStores<ApplicationDbContext>();

//faço com que por padrao as rotas sejam protegidas
builder.Services.AddAuthorization(options =>
{
  options.FallbackPolicy = new AuthorizationPolicyBuilder()
  .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
  .RequireAuthenticatedUser()
  .Build();
  options.AddPolicy("EmployeePolicy", policy => policy.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));
});
builder.Services.AddAuthentication(config =>
{
  config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


}).AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters()
  {
    ValidateActor = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ClockSkew = TimeSpan.Zero,
    ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
    ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
    IssuerSigningKey =
      new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]))
  };
});

//diz para o aspnet que vou usar essa classe como um serviço
builder.Services.AddScoped<QueryAllUserWithClaimNameService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMethods(CategoryPost.template, CategoryPost.methods, CategoryPost.handle);
app.MapMethods(CategoryGetAll.template, CategoryGetAll.methods, CategoryGetAll.handle);
app.MapMethods(CategoryPut.template, CategoryPut.methods, CategoryPut.handle);
app.MapMethods(EmployeePost.template, EmployeePost.methods, EmployeePost.handle);
app.MapMethods(EmployeeGetAll.template, EmployeeGetAll.methods, EmployeeGetAll.handle);
app.MapMethods(TokenPost.template, TokenPost.methods, TokenPost.handle);


app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{

  var error = http.Features?.Get<IExceptionHandlerFeature>().Error;

  if (error != null)
  {
    if (error is SqlException)
    {
      return Results.Problem(title: "Database out", statusCode: 500);
    }
  }

  return Results.Problem(title: "An error ocurred", statusCode: 500);
});

app.Run();
