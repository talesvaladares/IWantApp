using Flunt.Validations;

public class Category : Entity

{

  public Category(string name, string createdBy, string editedBy)
  {

    this.name = name;
    this.active = true;
    this.createdBy = createdBy;
    this.editedBy = editedBy;
    this.created_at = DateTime.Now;
    this.updated_at = DateTime.Now;

    validate();


  }
  public string name { get; private set; }
  public bool active { get; private set; } = true;

  private void validate()
  {
    var contratic = new Contract<Category>()
     .IsNotNullOrEmpty(name, "name", "name is required!")
     .IsGreaterOrEqualsThan(name, 3, "name")
     .IsNotNullOrEmpty(createdBy, "createdBy", "createdBy is required!")
     .IsNotNullOrEmpty(editedBy, "editedBy", "editedBy is required!");

    AddNotifications(contratic);
  }

  public void editInfo(string name, bool active, string editedBy)
  {
    this.active = active;
    this.name = name;
    this.editedBy = editedBy;
    this.updated_at = DateTime.Now;

    validate();

  }

}