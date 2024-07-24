FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY TelegramNewsAggregator/TelegramNewsAggregator.csproj TelegramNewsAggregator/

RUN dotnet restore TelegramNewsAggregator/TelegramNewsAggregator.csproj

COPY . .

RUN dotnet publish -c Release -o /app TelegramNewsAggregator/TelegramNewsAggregator.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80/tcp
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet",  "TelegramNewsAggregator.dll"]