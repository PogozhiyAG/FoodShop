use [FoodShop.Api.Order]

delete [dbo].[OrderAmountCorrections]
insert OrderAmountCorrections([Scope], [Priority], [TokenTypeCode], [Expression], [TypeCode])
select 
  [Scope] = 'VOLUME', 
  [Priority] = 100, 
  [TokenTypeCode] = null, 
  [Expression] = 'ctx => -0.07M * (ctx.SumOf("P", "PD") >= 100 ? ctx.SumOf("P", "PD") : 0)',   
  [TypeCode] = 'OVD'
union all select 
  [Scope] = 'DELIVERY', 
  [Priority] = 200, 
  [TokenTypeCode] = 'Club', 
  [Expression] = 'ctx => -1 * ctx.SumOf("D")',   
  [TypeCode] = 'DD'

select * from [dbo].[OrderAmountCorrections]


select * from Orders
select * from OrderItems
select * , sum(amount)over(partition by OrderId) from OrderCalculations
select * from OrderCalculationProperties
select * from DeliveryInfos
select * from OrderPaymentIntents


/*

delete Orders
delete OrderItems
delete OrderCalculations
delete OrderCalculationProperties
delete DeliveryInfos
*/


