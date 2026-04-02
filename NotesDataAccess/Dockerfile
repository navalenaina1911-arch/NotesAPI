# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy everything (your migrator project + shared code)
COPY . .

# Restore only the migrator project
RUN dotnet restore NotesDataAccess.csproj

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"


# Build the migrator project
RUN dotnet build NotesDataAccess.csproj -c Release -o /app/build

# Runtime stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS runtime
WORKDIR /app

# Install PostgreSQL client tools (pg_isready, psql, etc.)
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

# ✅ EF tools in runtime stage
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy built migrator
COPY --from=build /app .

# ✅ Restore packages in runtime stage
RUN dotnet restore NotesDataAccess.csproj

# Copy migration script
COPY run-migrations.sh /app/run-migrations.sh
RUN chmod +x /app/run-migrations.sh

# Run migrations when container starts
CMD ["./run-migrations.sh"]


