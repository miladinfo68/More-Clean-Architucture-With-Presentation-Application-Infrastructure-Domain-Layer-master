namespace CleanArch.Domain.DomainModels;

public interface IKeyId<TKey> where TKey : IEquatable<TKey>
{
    TKey Id { get; set; }
}


public abstract class BaseEntity: IKeyId<int>
{
    public int Id { get; set; }
    // public DateTimeOffset DateCreated { get; set; }
    // public DateTimeOffset? DateUpdated { get; set; }
    // public DateTimeOffset? DateDeleted { get; set; }
}
