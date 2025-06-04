namespace CarDDD.DomainServices.EntityObjects;

public class ManagerEntity : Entity<Guid>
{
    public string EmailAddress { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}