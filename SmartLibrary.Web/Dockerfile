FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SmartLibrary.Web/SmartLibrary.Web.csproj", "SmartLibrary.Web/"]
COPY ["SmartLibrary.Infrastructure/SmartLibrary.Infrastructure.csproj", "SmartLibrary.Infrastructure/"]
COPY ["SmartLibrary.Application/SmartLibrary.Application.csproj", "SmartLibrary.Application/"]
RUN dotnet restore "SmartLibrary.Web/SmartLibrary.Web.csproj"
COPY . .
WORKDIR "/src/SmartLibrary.Web"
RUN dotnet build "SmartLibrary.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SmartLibrary.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartLibrary.Web.dll"]
