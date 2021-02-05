#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/ExchangeRatesMcr.HistoryApp/ExchangeRatesMcr.HistoryApp.csproj", "src/ExchangeRatesMcr.HistoryApp/"]
RUN dotnet restore "src/ExchangeRatesMcr.HistoryApp/ExchangeRatesMcr.HistoryApp.csproj"
COPY . .
WORKDIR "/src/src/ExchangeRatesMcr.HistoryApp"
RUN dotnet build "ExchangeRatesMcr.HistoryApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExchangeRatesMcr.HistoryApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExchangeRatesMcr.HistoryApp.dll"]