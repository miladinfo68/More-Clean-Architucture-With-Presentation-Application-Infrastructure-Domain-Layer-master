using CleanArch.Domain.DomainModels;

namespace CleanArch.Domain.Abstractions.Interfaces.Services;

public interface IBaseService<T, TU> where T : BaseEntity where TU : class
{
   ValueTask<IEnumerable<TU>?> Get(CancellationToken cancellationToken = default);

    public ValueTask<IEnumerable<TU>?> GetMany<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public ValueTask<TU?> Get<TKey>(TKey id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

}



