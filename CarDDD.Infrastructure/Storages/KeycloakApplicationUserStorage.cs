using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;
using CarDDD.Settings.KeycloakSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarDDD.Infrastructure.Storages;

public class TokenResponse
{
    public string access_token { get; set; }
    public int    expires_in   { get; set; }
    public string token_type   { get; set; }
}

public class KeycloakUser
{
    public string   Id         { get; set; }
    public string   Username   { get; set; }
    public string   Email      { get; set; }
    public bool     Enabled    { get; set; }
    public IDictionary<string, object> Attributes { get; set; }
}

public class KeycloakApplicationUserStorage(ILogger<KeycloakApplicationUserStorage> log, IOptions<KeycloakSettings> settings) : IApplicationUserStorage
{
    private readonly HttpClient _httpClient = new();

    private async Task<string> GetAdminTokenAsync()
    {
        var form = new Dictionary<string, string>
        {
            ["grant_type"]    = "password",
            ["client_id"]     = settings.Value.ClientId,
            ["username"]      = settings.Value.AdminLogin,
            ["password"]      = settings.Value.AdminPassword,
        };

        var resp = await _httpClient.PostAsync(
            $"{settings.Value.KeycloakBaseUrl}/realms/{settings.Value.Realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(form)
        );
        
        if (!resp.IsSuccessStatusCode)
            return "";

        var payload = await JsonSerializer.DeserializeAsync<TokenResponse>(
            await resp.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        if (payload == null)
            return "";
        
        return payload.access_token;
    }

    public async Task<ApplicationUser?> ReadAsync(Guid userId)
    {
        var id = userId.ToString();
        
        var token = await GetAdminTokenAsync();
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await _httpClient.GetAsync($"{settings.Value.KeycloakBaseUrl}/realms/{settings.Value.Realm}/users/{id}");
        if (!resp.IsSuccessStatusCode)
            return null;
        
        var json = await resp.Content.ReadAsStringAsync();

        var keycloakUser = JsonSerializer.Deserialize<KeycloakUser>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        if (keycloakUser == null)
            return null;

        return new ApplicationUser
        {
            Id = Guid.Parse(keycloakUser.Id),
            Email = keycloakUser.Email,
            FirstName = keycloakUser.Attributes["firstName"].ToString() ?? string.Empty,
            LastName = keycloakUser.Attributes["lastName"].ToString() ?? string.Empty,
        };
    }
}