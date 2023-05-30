namespace CleanArch.Application.Dtos;

//mapping not allowed by functional syntaxt
//public  record class TradeDto(int Id, string Name, DateTime DateEn, decimal Open, decimal Close, decimal High, decimal Low);

public record TradeDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public DateTime DateEn { get; init; }
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
}










































//https://stackoverflow.com/questions/9218847/how-do-i-handle-database-connections-with-dapper-in-net