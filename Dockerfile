FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

COPY *.sln .
COPY Entities/*.csproj ./Entities/
COPY MessageBroker/*.csproj ./MessageBroker/
COPY Publisher/*.csproj ./Publisher/
COPY Reader/*.csproj ./Reader/
COPY Repository/*.csproj ./Repository/
COPY Services/*.csproj ./Services/
COPY Shared/*.csproj ./Shared/
COPY Summarizer/*.csproj ./Summarizer/
COPY Aggregator/*.csproj ./Aggregator/

COPY nuget.config ./

RUN dotnet restore --configfile nuget.config

COPY . .

WORKDIR /source/Aggregator

# Not using --no-restore because it causes NETSDK1064 (https://github.com/dotnet/core/issues/4512)
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

ENV api_hash="<api_hash>"
ENV api_id="<api_id>"
ENV bot_token="<bot_token>"
ENV openai_token="<openai_token>"
ENV phone_number="<phone_number>"

EXPOSE 8080

ENTRYPOINT ["dotnet", "Aggregator.dll"]
