using System.Security.Claims;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.Api.Extensions;

public static class EmployerRolesExtensions
{
    public static IReadOnlyList<Role> GetEmployersRoles(this ClaimsPrincipal user)
    {
        var roles = user.FindAll(ClaimTypes.Role)
            .Select(c => Enum.TryParse<Role>(c.Value, out var r) 
                ? r 
                : throw new InvalidOperationException($"Unknown role: {c.Value}"))
            .ToList();
        
        return roles;
    }
}