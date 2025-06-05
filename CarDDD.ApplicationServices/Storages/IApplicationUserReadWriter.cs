using CarDDD.ApplicationServices.Models.StorageModels;

namespace CarDDD.ApplicationServices.Storages;

public interface IApplicationUserReader
{
    public Task<ApplicationUser?> ReadOnceAsync(Guid userId);
}