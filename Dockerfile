FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["03.Presentation/GearStore.Web/GearStore.Web.csproj", "03.Presentation/GearStore.Web/"]
COPY ["01.Core/GearStore.Application/GearStore.Application.csproj", "01.Core/GearStore.Application/"]
COPY ["01.Core/GearStore.Domain/GearStore.Domain.csproj", "01.Core/GearStore.Domain/"]
COPY ["02.Infrastructure/GearStore.Infrastructure/GearStore.Infrastructure.csproj", "02.Infrastructure/GearStore.Infrastructure/"]

RUN dotnet restore "03.Presentation/GearStore.Web/GearStore.Web.csproj"

COPY . .
WORKDIR "/src/03.Presentation/GearStore.Web"
RUN dotnet build "GearStore.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GearStore.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "GearStore.Web.dll"]
