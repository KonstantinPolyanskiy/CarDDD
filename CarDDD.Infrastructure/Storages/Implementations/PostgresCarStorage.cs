using CarDDD.Core.AnswerObjects.Result;
using CarDDD.Core.DtoObjects;
using CarDDD.Core.SnapshotModels;
using CarDDD.Infrastructure.Contexts;
using CarDDD.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarDDD.Infrastructure.Storages.Implementations;

public class PostgresCarStorage(ApplicationDbContext database, ILogger<PostgresCarStorage> log) : ICarStorage
{
    public async Task<bool> SaveCarSnapshotAsync(CarSnapshot snapshot)
    {
        try
        {
            await database.Cars.AddAsync(snapshot);
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateCarSnapshot(CarSnapshot newSnapshot)
    {
        try
        {
            var snapshot = await database.Cars.FindAsync(newSnapshot.Id);
            if (snapshot == null)
                return false;
            
            snapshot.Brand = newSnapshot.Brand;
            snapshot.Color = newSnapshot.Color;
            snapshot.Price = newSnapshot.Price;
            
            snapshot.PhotoId = newSnapshot.PhotoId;
            snapshot.ManagerId = newSnapshot.ManagerId;
            
            snapshot.PreviousOwnerName = newSnapshot.PreviousOwnerName;
            snapshot.Mileage = newSnapshot.Mileage;
            snapshot.Condition = newSnapshot.Condition;
            snapshot.IsAvailable = newSnapshot.IsAvailable;
            
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<CarSnapshot?> GetCarSnapshotAsync(Guid carId)
    {
        try
        {
            return await database.Cars.FindAsync(carId);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<PageResult<CarSnapshot>?> GetCarSnapshotsAsync(SearchCarDto dto)
    {
        try
        {
            var q = database.Cars.AsNoTracking()
                .FilterByBrands(dto.Brands)
                .FilterByColors(dto.Colors)
                .FilterByCondition(dto.Condition)
                .OnlyAvailable(dto.OnlyAvailable);

            var total = await q.CountAsync();

            var page = dto.PageNumber < 1 ? 1 : dto.PageNumber;
            var size = dto.PageSize < 1 ? 10 : dto.PageSize;

            var snaps = await q.OrderBy(c => c.Brand)
                .ThenBy(c => c.Price)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
            
            return await PageResult<CarSnapshot>.CreateAsync(snaps.AsQueryable(), dto.PageNumber, dto.PageSize);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return null;
        }
    }
}