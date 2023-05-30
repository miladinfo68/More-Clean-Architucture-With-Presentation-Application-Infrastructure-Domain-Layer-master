using CleanArch.Domain.DomainModels;

namespace CleanArch.Domain.Abstractions.Interfaces.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<IReadOnlyCollection<T>?> Get(CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<T>?> GetMany<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<T?> Get<TKey>(TKey id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

}
