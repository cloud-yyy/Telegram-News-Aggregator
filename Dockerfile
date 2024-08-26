FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

COPY . .

RUN dotnet nuget add source "https://pkgs.dev.azure.com/tgbots/Telegram.Bot/_packaging/release/nuget/v3/index.json" -n Telegram.Bot

RUN dotnet restore

WORKDIR /source/TelegramNewsAggregator

RUN dotnet publish -c release -o ../publish --no-restore

###

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as production

WORKDIR /app

COPY --from=build source/publish ./

ENTRYPOINT ["dotnet", "TelegramNewsAggregator.dll"]
