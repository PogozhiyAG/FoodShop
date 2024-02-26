using FoodShop.Api.Order.Dto;
using FoodShop.Api.Order.Model;
using FoodShop.Api.Order.Services.Calculation;
using MediatR;
using Stripe;

namespace FoodShop.Api.Order.Commands;

public class CreateCheckoutCommand : IRequest<CreateCheckoutCommandResult>
{
    public required string UserId { get; set; }
    public required CreateOrderRequest CreateOrderRequest{ get; set; }
}


public class CreateCheckoutCommandResult
{
    public Model.Order? Order { get; set; }
    public PaymentIntent? PaymentIntent { get; set; }
    public OrderPaymentIntent? OrderPaymentIntent { get; set; }
    public OrderCalculationContext? OrderCalculationContext { get; set; }
}