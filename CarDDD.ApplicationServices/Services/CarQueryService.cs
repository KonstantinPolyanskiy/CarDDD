using AutoMapper;
using CarDDD.ApplicationServices.Models.AnswerObjects.Result;
using CarDDD.ApplicationServices.Models.AnswerObjects.ServiceResponses;
using CarDDD.ApplicationServices.Repositories;
using CarDDD.ApplicationServices.Services.Helpers;
using CarDDD.ApplicationServices.Storages;
using CarDDD.DomainServices.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CarDDD.ApplicationServices.Services;

/// <summary>
/// Сервис для получения информации о доменных машинах
/// </summary>
public interface ICarQueryService
{
    public Task<Result<CarInfo>> CarByIdAsync(Guid carId);
    
}

public class CarQueryService(ICarRepositoryReader carsReader, IPhotoStorage photos, IMapper mapper) : ICarQueryService
{
    public async Task<Result<CarInfo>> CarByIdAsync(Guid carId)
    {
        var car = await carsReader.GetAsync(CarId.From(carId));
        if (car == null)
            return Result<CarInfo>.Failure(Error.Application(ErrorType.NotFound, $"car by id:{carId} not found"));

        var result = mapper.Map<CarInfo>(car);

        if (car.Photo.Attached())
            result.PhotoUrl = await photos.DownloadUrlAsync(car.Photo.Id);
        
        return Result<CarInfo>.Success(result);
    }

    public async Task<Result<FiltratedCarsInfo>> CarByIdFiltersAsync(CarFilters criteria)
    {
        var carQuery = await carsReader.CarsQueryAsync();
        
        if (criteria.Brands.Any())
            carQuery = carQuery.Where(s => criteria.Brands.Contains(s.Brand));
        
        if (criteria.Colors.Any())
            carQuery = carQuery.Where(s => criteria.Colors.Contains(s.Color));
        
        if (criteria.Conditions.Any())
        {
            var conditions = ConditionParser.Parse(criteria.Conditions);
            if (conditions.Any())
                carQuery = carQuery.Where(s => conditions.Contains(s.Condition));
        }
        
        if (criteria.PriceRange.HasValue)
        {
            var (min, max) = criteria.PriceRange.Value;
            if (min.HasValue) carQuery = carQuery.Where(c => c.Price >= min.Value);
            if (max.HasValue) carQuery = carQuery.Where(c => c.Price <= max.Value);
        }

        var allFiltratedCars = await carQuery.ToListAsync();
        var totalCount = allFiltratedCars.Count;

        var skip = (Math.Max(criteria.PageNumber, 1) - 1) * criteria.PageSize;
        
        var pagedCars = allFiltratedCars
            .Skip(skip)
            .Take(criteria.PageSize)
            .ToList();
        
        var availableFilters = new CarFilters
        {
            Brands     = allFiltratedCars.Select(c => c.Brand).Distinct().OrderBy(x => x).ToList(),
            Colors     = allFiltratedCars.Select(c => c.Color).Distinct().OrderBy(x => x).ToList(),
            Conditions = allFiltratedCars.Select(c => c.Condition.ToString())  
                .Distinct()
                .OrderBy(s => s)
                .ToList(),
            PriceRange = allFiltratedCars.Any()
                ? (allFiltratedCars.Min(c => c.Price), allFiltratedCars.Max(c => c.Price))
                : (0m, 0m)
        };

        return Result<FiltratedCarsInfo>.Success(new FiltratedCarsInfo
        {
            Cars = pagedCars.Select(mapper.Map<CarInfo>).ToList(),
            
            AvailableFilters = availableFilters,
            PageNumber = criteria.PageNumber,
            PageSize = criteria.PageSize,
            TotalCount = totalCount
        });
    }
}