namespace FoodShop.Api.Catalog.Dto;

public class OfferedProductBatchDto : OfferedProductDto
{
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal OfferAmount { get; set; }
}
