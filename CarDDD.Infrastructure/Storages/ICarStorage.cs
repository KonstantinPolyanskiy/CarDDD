using CarDDD.Core.AnswerObjects.Result;
using CarDDD.Core.DtoObjects;
using CarDDD.Core.SnapshotModels;

namespace CarDDD.Infrastructure.Storages;

public interface ICarStorage
{
    public Task<bool> SaveCarSnapshotAsync(CarSnapshot snapshot);
    public Task<bool> UpdateCarSnapshot(CarSnapshot snapshot);
    
    public Task<CarSnapshot?> GetCarSnapshotAsync(Guid carId);
    public Task<PageResult<CarSnapshot>?> GetCarSnapshotsAsync(SearchCarDto dto);
}