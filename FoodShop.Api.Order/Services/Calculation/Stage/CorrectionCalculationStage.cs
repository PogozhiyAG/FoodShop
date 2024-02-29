using FoodShop.Api.Order.Model;
using System.Security.Claims;

namespace FoodShop.Api.Order.Services.Calculation.Stage;

public class CorrectionCalculationStage : IOrderCalculationStage
{
    public const string DEFAULT_SERVICE_KEY = "correction";

    public const string PROPERTY_CORRECTION = "C";

    private readonly IOrderAmountCorrectionsProvider _orderAmountCorrectionsProvider;
    private readonly ICustomerProfile _customerProfile;
    private readonly IAuthenticationContext _authenticationContext;

    public CorrectionCalculationStage(IOrderAmountCorrectionsProvider orderAmountCorrectionsProvider, ICustomerProfile customerProfile, IAuthenticationContext authenticationContext)
    {
        _orderAmountCorrectionsProvider = orderAmountCorrectionsProvider;
        _customerProfile = customerProfile;
        _authenticationContext = authenticationContext;
    }

    public async Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext)
    {
        var tokenTypes = await GetTokenTypes();
        var corrections = await _orderAmountCorrectionsProvider.GetCorrections(tokenTypes);
        return EnumerateCalculations(orderCalculationContext, corrections);

    }

    private IEnumerable<OrderCalculation> EnumerateCalculations(OrderCalculationContext orderCalculationContext, IEnumerable<OrderAmountCorrection> corrections)
    {
        foreach (var item in corrections)
        {
            var calculation = orderCalculationContext.CreateCalculation(c =>
            {
                c.TypeCode = item.TypeCode;
                c.Amount = item.GetExpression()(orderCalculationContext);
            });

            calculation.Properties.Add(new OrderCalculationProperty() { Name = PROPERTY_CORRECTION, Value = item.Id.ToString() });

            yield return calculation;
        }
    }

    private async Task<IEnumerable<string>> GetTokenTypes()
    {
        var principal = _authenticationContext.User;
        var isAnonymous = principal.Claims.Any(c => c.Type == ClaimTypes.Anonymous);
        if (isAnonymous)
        {
            return Array.Empty<string>();
        }
        //TODO: JWT delegating?
        return await _customerProfile.GetTokenTypes(principal.Identity.Name);
    }
}
