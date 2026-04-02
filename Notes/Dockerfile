# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore Notes/Notes.csproj
RUN dotnet publish Notes/Notes.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Explicitly copy appsettings files
COPY --from=build /app/Notes/appsettings.json .
COPY --from=build /app/Notes/appsettings.Development.json .

# Controls which appsettings.{ENV}.json is loaded
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

ENTRYPOINT ["dotnet", "Notes.dll"]