
SELECT  *  FROM [FoodShop].[dbo].[Products]


sp_spaceused 'dbo.Products'

sp_help 'dbo.Products'




  insert [dbo].[RankingTypes] (id, Code, Description)
  select newid(),  'Popularity', 'Popularity'




  select * from [dbo].[ProductPriceStrategyLinks]

  select * from ProductCategories


  set statistics io, time on

  SELECT *  
  FROM [FoodShop].[dbo].[Products] 
  where CategoryId in (select top 50 Id from ProductCategories order by name )
  order by Popularity desc
  OFFSET     0 ROWS       -- skip 10 rows
  FETCH NEXT 50 ROWS ONLY; -- take 10 rows


  SELECT *  
  FROM [FoodShop].[dbo].[Products] 
  where CategoryId =98--in (select top 5 Id from ProductCategories order by name )
  order by Popularity desc
  OFFSET     0 ROWS       -- skip 10 rows
  FETCH NEXT 50 ROWS ONLY; -- take 10 rows


    SELECT *  
  FROM [FoodShop].[dbo].[Products] 
  where BrandId = 410--in (select top 5 Id from ProductCategories order by name )
  order by Popularity desc
  OFFSET     1000 ROWS       -- skip 10 rows
  FETCH NEXT 50 ROWS ONLY; -- take 10 rows


  
    SELECT BrandID, cnt = count(*)
  FROM [FoodShop].[dbo].[Products] 
  group by BrandID
  order by cnt desc


  select BrandId, cnt = count(*)
  from(
   SELECT *  
  FROM [FoodShop].[dbo].[Products]   
  order by CustomerRating desc
  OFFSET     0 ROWS       -- skip 10 rows
  FETCH NEXT 1000 ROWS ONLY
  )t
  group by BrandId



  SELECT BrandID, sum(CustomerRating), count (*), a = sum(CustomerRating) / count(*)
  FROM [FoodShop].[dbo].[Products]   
  group by BrandId
  having count(*) > 10
  order by a desc