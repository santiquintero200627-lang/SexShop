FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Rutas limpias asumiendo que el Dockerfile está en la raíz
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
RUN mkdir -p /app/app_data
EXPOSE 8080
ENTRYPOINT ["dotnet", "SexShop.API.dll"]