namespace CarDDD.ApplicationServices.Models.StorageModels;

public class ApplicationUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "manager@mail.com";
    public string FirstName { get; set; } = "Test FirstName";
    public string LastName { get; set; } = "Test LastName";
}