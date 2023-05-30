using System.Data;
using CleanArch.Domain.DomainModels;

namespace CleanArch.Domain.Abstractions.Interfaces.Repositories;

public abstract class BaseRepository<T> : IBaseRepository<T> where T: BaseEntity
{
    protected IDbConnection DbConnection { get;}
    protected BaseRepository(IDbConnection dbConnection)=> DbConnection = dbConnection;


    public abstract Task<IReadOnlyCollection<T>?> Get(CancellationToken cancellationToken = default);

}