using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.DtoObjects;
using CarDDD.Core.SnapshotModels;

namespace CarDDD.Infrastructure.Repositories.Implementations;

internal static class CarSnapshotFiltering
{
    public static IQueryable<CarSnapshot> FilterByBrands(
        this IQueryable<CarSnapshot> q, IEnumerable<string>? brands)
        => brands is null || !brands.Any()
            ? q
            : q.Where(c => brands.Contains(c.Brand));

    public static IQueryable<CarSnapshot> FilterByColors(
        this IQueryable<CarSnapshot> q, IEnumerable<string>? colors)
        => colors is null || !colors.Any()
            ? q
            : q.Where(c => colors.Contains(c.Color));

    public static IQueryable<CarSnapshot> FilterByCondition(
        this IQueryable<CarSnapshot> q, Condition? condition)
        => condition is null
            ? q
            : q.Where(c => c.Condition == condition);

    public static IQueryable<CarSnapshot> FilterByPrice(
        this IQueryable<CarSnapshot> q, decimal? min, decimal? max)
    {
        if (min is not null) q = q.Where(c => c.Price >= min);
        if (max is not null) q = q.Where(c => c.Price <= max);
        return q;
    }

    public static IQueryable<CarSnapshot> OnlyAvailable(
        this IQueryable<CarSnapshot> q, bool onlyAvailable)
        => onlyAvailable ? q.Where(c => c.IsAvailable) : q;
}