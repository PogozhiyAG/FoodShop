#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FoodShop.Api.Catalog/FoodShop.Api.Catalog.csproj", "FoodShop.Api.Catalog/"]
RUN dotnet restore "FoodShop.Api.Catalog/FoodShop.Api.Catalog.csproj"
COPY . .
WORKDIR "/src/FoodShop.Api.Catalog"
RUN dotnet build "FoodShop.Api.Catalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodShop.Api.Catalog.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodShop.Api.Catalog.dll"]