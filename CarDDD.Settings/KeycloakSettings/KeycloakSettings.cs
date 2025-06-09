namespace CarDDD.Settings.KeycloakSettings;

public class KeycloakSettings
{
    public string KeycloakBaseUrl { get; set; } = string.Empty;
    
    public string Realm { get; set; } = string.Empty;
    
    public string ClientId { get; set; } = string.Empty;
    
    public string ClientSecret { get; set; } = string.Empty;
    
    public string AdminLogin { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
}