FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Dockerfile para la API
COPY ["SexShop.API/SexShop.API.csproj", "SexShop.API/"]
COPY ["SexShop.Application/SexShop.Application.csproj", "SexShop.Application/"]
COPY ["SexShop.Domain/SexShop.Domain.csproj", "SexShop.Domain/"]
COPY ["SexShop.Infrastructure/SexShop.Infrastructure.csproj", "SexShop.Infrastructure/"]

RUN dotnet restore "SexShop.API/SexShop.API.csproj"

COPY . .
WORKDIR "/src/SexShop.API"
RUN dotnet publish "SexShop.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "SexShop.API.dll"]