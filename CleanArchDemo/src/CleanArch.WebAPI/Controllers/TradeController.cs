using CleanArch.Application.Dtos;
using CleanArch.Application.Interfaces.Services;
using CleanArch.Common.Formater;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradeController : ControllerBase
{

    private readonly ILogger<TradeController> _logger;
    private readonly ITradeService _tradeService;
    public TradeController(ILogger<TradeController> logger, ITradeService tradeService)
    {
        _logger = logger;
        _tradeService = tradeService;
    }

    [HttpGet]
    public string Hc() => "health check is ok Now Call http://localhost:5029/api/trade/lasttrades";



    [HttpGet("LastTrades")]
    public async Task<ActionResult<IEnumerable<TradeDto>>> GetAll(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IEnumerable<TradeDto>? lastTrades = await _tradeService.Get(cancellationToken);
        return Ok(lastTrades);
    }

    [HttpGet("GetOne/{id}")]
    public async Task<ActionResult<IEnumerable<TradeDto>>> GetOne(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
       TradeDto? trade = await _tradeService.Get(id,cancellationToken);
        return Ok(trade);
    }

    [HttpPost("GetMany")]
    public async Task<ActionResult<IEnumerable<TradeDto>>> GetMany(int[] ids,  CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IEnumerable<TradeDto>? lastTrades = await _tradeService.GetMany(ids,cancellationToken);
        return Ok(lastTrades);
    }





    //[HttpGet("LastTrades")]
    //public async Task<ApiResponse<IEnumerable<TradeDto>>> Get(CancellationToken cancellationToken)
    //{
    //    cancellationToken.ThrowIfCancellationRequested();
    //    IEnumerable<TradeDto>? lastTrades = await _tradeService.Get(cancellationToken);
    //    return ApiResponse<IEnumerable<TradeDto>>.SuccessResponse(lastTrades, 200, $"[{lastTrades?.Count() ?? 0 }] records Geted");
    //}



    //comment NotFound in try block GlobalErrorHandlingMiddleware and fire these exceptions

    //[HttpGet("Null")]
    //public IActionResult Null() => throw new ResponseNullException("this error is due to null exception");

    //[HttpGet("Forbiden")]
    //public IActionResult Forbiden() => throw new ResponseNullException("this error is due to access deny exception");

}



