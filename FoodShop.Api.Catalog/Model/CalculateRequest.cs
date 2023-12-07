namespace FoodShop.Api.Catalog.Model
{
    public class CalculateRequest
    {
        public Dictionary<string, int> Items { get; set; } = new();
    }
}
