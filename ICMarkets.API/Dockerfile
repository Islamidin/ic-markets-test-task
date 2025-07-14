# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ICMarkets.API/ICMarkets.API.csproj", "ICMarkets.API/"]
COPY ["ICMarkets.Application/ICMarkets.Application.csproj", "ICMarkets.Application/"]
COPY ["ICMarkets.Domain/ICMarkets.Domain.csproj", "ICMarkets.Domain/"]
COPY ["ICMarkets.Infrastructure/ICMarkets.Infrastructure.csproj", "ICMarkets.Infrastructure/"]
COPY ["ICMarkets.Persistence/ICMarkets.Persistence.csproj", "ICMarkets.Persistence/"]
RUN dotnet restore "./ICMarkets.API/ICMarkets.API.csproj"
COPY . .
WORKDIR "/src/ICMarkets.API"
RUN dotnet build "./ICMarkets.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ICMarkets.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ICMarkets.API.dll"]