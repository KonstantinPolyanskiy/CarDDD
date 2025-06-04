namespace CarDDD.DomainServices.ValueObjects;

public readonly record struct Photo
{
    public Guid Id { get; init; }

    /// <summary>
    /// Расширение файла (".jpg", ".png")
    /// </summary>
    public string Extension { get; init; }

    public Photo(Guid id, string extension)
    {
        Id = id;
        Extension = extension;
    }
    
    /// <summary>
    /// Не прикрепленное (пустое) фото
    /// </summary>
    public static Photo None => new(Guid.Empty, string.Empty);

    public static Photo CreateNew(string extension)
    {
        // Пустое или неправильное расширение -> фото не прикреплено
        if (string.IsNullOrWhiteSpace(extension) || !extension.StartsWith("."))
            return None;

        return new Photo(Guid.NewGuid(), extension.Trim().ToLowerInvariant());
    }

    /// <summary> 
    /// Прикреплённое фото (Id не пустой и Extension не пустая).
    /// </summary>
    public bool Attached() => Id != Guid.Empty && !string.IsNullOrEmpty(Extension);
}