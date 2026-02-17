# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto para restaurar dependencias
COPY ["SexShop.API/SexShop.API.csproj", "SexShop.API/"]
COPY ["SexShop.Application/SexShop.Application.csproj", "SexShop.Application/"]
COPY ["SexShop.Domain/SexShop.Domain.csproj", "SexShop.Domain/"]
COPY ["SexShop.Infrastructure/SexShop.Infrastructure.csproj", "SexShop.Infrastructure/"]
RUN dotnet restore "SexShop.API/SexShop.API.csproj"

# Copiar el resto del código y publicar
COPY . .
RUN dotnet publish "SexShop.API/SexShop.API.csproj" -c Release -o /app/publish

# Etapa final de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Crear carpeta para que SQLite sea persistente en el disco de Render
RUN mkdir -p /app/data

# Exponer el puerto que Render usará
EXPOSE 8080

ENTRYPOINT ["dotnet", "SexShop.API.dll"]