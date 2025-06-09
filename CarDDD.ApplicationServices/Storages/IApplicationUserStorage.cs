using CarDDD.ApplicationServices.Models.StorageModels;

namespace CarDDD.ApplicationServices.Storages;

public interface IApplicationUserStorage
{
    public Task<ApplicationUser?> ReadAsync(Guid userId);
}