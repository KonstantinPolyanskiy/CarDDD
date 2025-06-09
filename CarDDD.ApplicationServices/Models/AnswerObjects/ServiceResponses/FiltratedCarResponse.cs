using CarDDD.DomainServices.DomainAggregates.CarAggregate;

namespace CarDDD.ApplicationServices.Models.AnswerObjects.ServiceResponses;

/// <summary>
/// Фильтры применимые к машинам
/// </summary>
public class CarFilters
{
    public List<string> Brands { get; set; } = new();
    public List<string> Colors { get; set; } = new();
    public (decimal? Min, decimal? Max)? PriceRange { get; set; } = null;
    public List<string> Conditions { get; set; } = new();
    
    public int PageNumber { get; set; } = 1;   
    public int PageSize   { get; set; } = 20;
}

public class FiltratedCarsInfo
{
    /// <summary>
    /// Доменные модели машин полученные согласно фильтрам
    /// </summary>
    public IReadOnlyList<CarInfo> Cars { get; set; } = [];
    
    /// <summary>
    /// Допустимые к применению фильтры
    /// </summary>
    public CarFilters AvailableFilters { get; set; } = new();
    
    
    public int PageNumber { get; set; }
    public int PageSize   { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}