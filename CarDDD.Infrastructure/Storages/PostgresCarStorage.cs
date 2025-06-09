using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;
using CarDDD.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarDDD.Infrastructure.Storages;

public class PostgresCarStorage(ApplicationDbContext database, ILogger<PostgresCarStorage> log) : ICarSnapshotStorage
{
    public async Task<CarSnapshot?> ReadAsync(Guid carId, CancellationToken ct = default)
    {
        try
        {
            return await database.Cars.FindAsync([carId, ct], cancellationToken: ct);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return null;
        }
    }

    public Task<IQueryable<CarSnapshot>> QueryForReadAllAsync(CancellationToken ct = default)
    {
        return Task.FromResult(database.Cars.AsQueryable());
    }

    public async Task<bool> SaveAsync(CarSnapshot carSnapshot, CancellationToken ct = default)
    {
        try
        {
            await database.Cars.AddAsync(carSnapshot, ct);
            await database.SaveChangesAsync(ct);
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateAsync(CarSnapshot newSnapshot, CancellationToken ct = default)
    {
        try
        {
            var snapshot = await database.Cars.FindAsync([newSnapshot.Id, ct], cancellationToken: ct);
            if (snapshot == null)
                return false;
            
            snapshot.Brand = newSnapshot.Brand;
            snapshot.Color = newSnapshot.Color;
            snapshot.Price = newSnapshot.Price;
            
            snapshot.PhotoId = newSnapshot.PhotoId;
            snapshot.ManagerId = newSnapshot.ManagerId;
            
            snapshot.Mileage = newSnapshot.Mileage;
            snapshot.IsAvailable = newSnapshot.IsAvailable;
            
            await database.SaveChangesAsync(ct);
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }
}