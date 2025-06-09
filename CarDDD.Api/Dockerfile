FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CarDDD.Api/CarDDD.Api.csproj", "CarDDD.Api/"]
COPY ["CarDDD.Settings/CarDDD.Settings.csproj", "CarDDD.Settings/"]
COPY ["CarDDD.Infrastructure/CarDDD.Infrastructure.csproj", "CarDDD.Infrastructure/"]
COPY ["CarDDD.ApplicationServices/CarDDD.ApplicationServices.csproj", "CarDDD.ApplicationServices/"]
COPY ["CarDDD.Contracts/CarDDD.Contracts.csproj", "CarDDD.Contracts/"]
COPY ["CarDDD.DomainServices/CarDDD.DomainServices.csproj", "CarDDD.DomainServices/"]
RUN dotnet restore "CarDDD.Api/CarDDD.Api.csproj"
COPY . .
WORKDIR "/src/CarDDD.Api"
RUN dotnet build "CarDDD.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CarDDD.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarDDD.Api.dll"]
