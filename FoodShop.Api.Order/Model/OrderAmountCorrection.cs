using FoodShop.Api.Order.Services.Calculation;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace FoodShop.Api.Order.Model;

public class OrderAmountCorrection
{
    public long Id { get; set; }
    public string? Scope { get; set; }
    public decimal Priority { get; set; }
    public string? TokenTypeCode { get; set; }
    public required string Expression { get; set; }
    public string TypeCode { get; set; }

    private Lazy<Func<OrderCalculationContext, decimal>> lazyExpression;

    public OrderAmountCorrection()
    {
        lazyExpression = new Lazy<Func<OrderCalculationContext, decimal>>(
            () => {
                var options = ScriptOptions.Default
                    .AddReferences(typeof(OrderCalculationContext).Assembly)
                    .AddImports("System.Linq");

                var result = CSharpScript.EvaluateAsync<Func<OrderCalculationContext, decimal>>(Expression, options).Result;
                return result;
            },
            true
        );
    }

    public Func<OrderCalculationContext, decimal> GetExpression() => lazyExpression.Value;
}
