using Flunt.Notifications;

public abstract class Entity : Notifiable<Notification>
{
  //construtor, já inicia com o id no instanciamento da classe herdada
  public Entity()
  {
    id = Guid.NewGuid();
  }

  public Guid id { get; set; }

  public string createdBy { get; set; }

  public string editedBy { get; set; }

  public DateTime created_at { get; set; }

  public DateTime updated_at { get; set; }
}