using CleanArch.Domain.Abstractions.Interfaces.Repositories;
using CleanArch.Domain.DomainModels;

namespace CleanArch.Domain.Abstractions.Interfaces.Services;

public abstract class BaseService<T, TU> : IBaseService<T, TU> where T : BaseEntity where TU : class
{
    protected IBaseRepository<T> BaseRepository { get; }

    protected BaseService(IBaseRepository<T> baseRepository)
    {
        BaseRepository = baseRepository;
    }

    public abstract ValueTask<IEnumerable<TU>?> Get(CancellationToken cancellationToken = default);

}



