FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY "TicketSystem.DiscordBot/*.csproj" "TicketSystem.DiscordBot/"
RUN dotnet restore TicketSystem.DiscordBot

COPY . .
RUN dotnet publish TicketSystem.DiscordBot -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /data
COPY --from=build /app/publish /app
ENTRYPOINT ["dotnet", "/app/TicketSystem.DiscordBot.dll"]