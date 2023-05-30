using CleanArch.Application.Dtos;
using CleanArch.Common.Mapping;
using CleanArch.Domain.Abstractions.Interfaces.Services;
using CleanArch.Domain.DomainModels;
using CleanArch.Infrastructure.Concretes.Repositories;

namespace CleanArch.Application.Interfaces.Services;


public interface ITradeService : IBaseService<Trade, TradeDto>
{

}



public class TradeService : ITradeService
{
    private readonly ITradeRepositoryDapper _tradeRepository;
    public TradeService(ITradeRepositoryDapper tradeRepository)
    {
        _tradeRepository = tradeRepository;
    }

    public async ValueTask<IEnumerable<TradeDto>?> Get(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IEnumerable<Trade>? trades = await _tradeRepository.Get(cancellationToken);
        IEnumerable<TradeDto>? tradeDtos = trades?.Mapper4<Trade, TradeDto>();
        return tradeDtos;
    }

    public async ValueTask<IEnumerable<TradeDto>?> GetMany<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IEnumerable<Trade>? trades = await _tradeRepository.GetMany(ids, cancellationToken);
        IEnumerable<TradeDto>? tradeDtos = trades?.Mapper4<Trade, TradeDto>();
        return tradeDtos;
    }

    public async ValueTask<TradeDto?> Get<TKey>(TKey id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Trade? trade = await _tradeRepository.Get(id, cancellationToken);
        TradeDto? tradeDto = trade?.Mapper<Trade, TradeDto>();
        return tradeDto;
    }

}

