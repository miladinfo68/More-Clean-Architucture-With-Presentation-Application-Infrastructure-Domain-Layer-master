namespace CleanArch.Domain.DomainModels;

public class Trade : BaseEntity
{
    public string? Name { get; set; }
    public DateTime DateEn { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
}
