FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ApiMonetizationGateway.API/ApiMonetizationGateway.API.csproj", "ApiMonetizationGateway.API/"]
COPY ["ApiMonetizationGateway.Core/ApiMonetizationGateway.Core.csproj", "ApiMonetizationGateway.Core/"]
COPY ["ApiMonetizationGateway.Infrastructure/ApiMonetizationGateway.Infrastructure.csproj", "ApiMonetizationGateway.Infrastructure/"]
COPY ["ApiMonetizationGateway.Services/ApiMonetizationGateway.Services.csproj", "ApiMonetizationGateway.Services/"]

RUN dotnet restore "ApiMonetizationGateway.API/ApiMonetizationGateway.API.csproj"

COPY . .
WORKDIR "/src/ApiMonetizationGateway.API"

RUN dotnet build "ApiMonetizationGateway.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiMonetizationGateway.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "ApiMonetizationGateway.API.dll"]
