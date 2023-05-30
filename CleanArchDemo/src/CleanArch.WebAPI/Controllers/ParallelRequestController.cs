using CleanArch.Application.Dtos;
using CleanArch.Application.Interfaces.Services;
using CleanArch.Common.Formater;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParallelRequestController : ControllerBase
{

    private readonly ILogger<ParallelRequestController> _logger;
    private readonly ITradeService _tradeService;
    private readonly IHttpClientFactory _httpClientFactory;
    public ParallelRequestController(
        ILogger<ParallelRequestController> logger, 
        ITradeService tradeService, 
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _tradeService = tradeService;
        _httpClientFactory = httpClientFactory;
    }


    [HttpGet]
    public string Hc() => "health check is ok Now Call http://localhost:5029/api/TestBulkRequest/GetOne/1";

    [HttpGet("{id}")]
    public async Task<TradeDto?> Get(int id,CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var item = await _tradeService.Get(id,cancellationToken);
        return item;
    }

    [HttpPost]
    public async Task<IEnumerable<TradeDto>?> Get(int[] ids, CancellationToken cancellationToken)
    {
        //IEnumerable<object> ids= idparams.Split(",").Select(s=>s);
        cancellationToken.ThrowIfCancellationRequested();
        IEnumerable<TradeDto>? items = await _tradeService.GetMany(ids ,cancellationToken);
        return items;
    }



}



