#!/bin/sh
echo "Waiting for Postgres..."
set -e
until pg_isready -h postgres -p 5432; do
  sleep 2
done
echo "Running migrations..."
dotnet ef database update --project NotesDataAccess.csproj
echo "Migrations complete."
