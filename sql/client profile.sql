use [FoodShop.Api.CustomerProfile]

delete CustomerTokenTypes
insert CustomerTokenTypes
          select 'XMAS', 'Christmas'
union all select 'Club', 'Black club membership'

delete CustomerTokens
insert [dbo].[CustomerTokens](id, UserName, [TokenTypeCode], [ValidFrom], [ValidTo])
select
  id              = newid(),
  [UserName]        = 'string', 
  [TokenTypeCode] = 'XMAS', 
  [ValidFrom]     = getdate(), 
  [ValidTo]       = getdate() + 90
union all
select
  id              = newid(),
  [UserName]        = 'string', 
  [TokenTypeCode] = 'Club', 
  [ValidFrom]     = getdate(), 
  [ValidTo]       = getdate() + 90


delete dbo.CustomerDeliveryInfos
insert dbo.CustomerDeliveryInfos([Id], [UserName], [Address], [PostCode], [ContactName], [ContactPhone])
select
  [Id]            = newid(), 
  [UserName]      = 'string', 
  [Address]       = 'Teddington, 22 Langdon Park, 3', 
  [PostCode]      = 'TW119FE', 
  [ContactName]   = 'Aleksander', 
  [ContactPhone]  = '+44123789456'
union all 
select
  [Id]            = newid(), 
  [UserName]      = 'string1', 
  [Address]       = 'Hampton Wick, Chirch grove, The Firs', 
  [PostCode]      = 'TW119FE', 
  [ContactName]   = 'Aleksander', 
  [ContactPhone]  = '+44123789456'


select * from CustomerTokenTypes
select * from CustomerTokens
select * from CustomerDeliveryInfos

  