#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FoodShop.Api.Order/FoodShop.Api.Order.csproj", "FoodShop.Api.Order/"]
RUN dotnet restore "./FoodShop.Api.Order/./FoodShop.Api.Order.csproj"
COPY . .
WORKDIR "/src/FoodShop.Api.Order"
RUN dotnet build "./FoodShop.Api.Order.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FoodShop.Api.Order.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodShop.Api.Order.dll"]