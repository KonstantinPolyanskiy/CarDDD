using Microsoft.EntityFrameworkCore;

namespace CarDDD.Core.AnswerObjects.Result;

/// <summary>
/// Результат постраничного поиска 
/// </summary>
public class PageResult<T>
{
    public IReadOnlyList<T> Items { get; }
    
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    
    private PageResult(IEnumerable<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        Items      = items.ToList().AsReadOnly();
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize   = pageSize;
    }
    
    public static async Task<PageResult<T>> CreateAsync(IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize   < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        var total = query.Count();
        var items =  await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PageResult<T>(items, total, pageNumber, pageSize);
    }
}