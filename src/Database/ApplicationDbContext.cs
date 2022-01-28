using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{

  public DbSet<Product> products { get; set; }

  public DbSet<Category> categories { get; set; }

  //construtor
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder builder)

  {
    base.OnModelCreating(builder);
    //digo ao builder do banco de dados que é para ignorar notification para ele nao 
    //tentar fazer nada no banco
    builder.Ignore<Notification>();

    builder.Entity<Product>().Property(p => p.description).HasMaxLength(225);

    builder.Entity<Product>().Property(p => p.name).IsRequired();

    builder.Entity<Category>().Property(c => c.name).IsRequired();

  }

  //fala que sempre que um campo do tipo string no banco de dados for criado
  //ele terá no máximo 100 caracteres
  protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
  {
    configuration.Properties<string>().HaveMaxLength(100);
  }
}