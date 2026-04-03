# 📝 Notes API

> A production-ready .NET 10 REST API built for **Traton Financial Services** — demonstrating scalable architecture, modern patterns, and operational best practices.

---

## ⚠️ Deployment Note

The API runs fully and correctly **locally**. In the deployed environment, PostgreSQL migrations had not been executed, meaning the database schema was never created. This is a **deployment configuration issue** — not an application bug — and is straightforward to resolve by running migrations as part of the CI/CD pipeline or startup process.

---

## 🏗️ Architecture Overview

The project follows a clean, layered architecture designed for maintainability and testability:

```
┌─────────────────────────────────────────┐
│              Controllers (Thin)          │  ← Route & delegate only
├─────────────────────────────────────────┤
│            Service Layer                 │  ← Business logic (CQRS)
├─────────────────────────────────────────┤
│          Repository Layer                │  ← Generic T pattern
├─────────────────────────────────────────┤
│         PostgreSQL (via EF Core)         │  ← Npgsql + Migrations
└─────────────────────────────────────────┘
```

### Design Principles

| Concern | Approach |
|---|---|
| **Pattern** | CQRS — Commands and Queries are separated |
| **Controllers** | Thin — delegate immediately to the service layer |
| **Business Logic** | Encapsulated in the Service layer |
| **Data Access** | Generic repository `IRepository<T>` for reusable CRUD |
| **Request/Response** | Strongly-typed DTOs using C# `record` types |
| **Validation** | [FluentValidation](https://docs.fluentvalidation.net/) on all request models |
| **Observability** | Structured telemetry middleware with consistent logging |
| **Resilience** | `CancellationToken` threaded through every layer |
| **API Versioning** | All endpoints prefixed with `/api/v1` |

---

## 🔌 Endpoints

Base path: `/api/v1/notes`

| Method | Route | Description | Request Body |
|---|---|---|---|
| `GET` | `/getAll` | Retrieve all notes | — |
| `GET` | `/{id}` | Retrieve a note by ID | — |
| `POST` | `/` | Create a new note | `CreateNoteRequestDTO` |
| `PUT` | `/{id}` | Update an existing note | `UpdateNoteRequestDTO` |
| `DELETE` | `/{id}` | Soft-delete a note | — |

---

## ✨ Features

### 🗑️ Soft Delete
Notes are never permanently removed. A boolean `IsDeleted` column flags deleted records, and EF Core's `HasQueryFilter` automatically excludes them from all queries — no manual filtering required anywhere in the codebase.

### 🔍 Full-Text Search
PostgreSQL's native full-text search is implemented using:
- A **`tsvector` generated column** (via `HasGeneratedTsVectorColumn`) that is kept in sync automatically
- A **GIN index** for fast keyword lookups at scale
- No external search infrastructure required

---

## 🛠️ Tech Stack

- **Runtime**: .NET 10
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core + Npgsql
- **Validation**: FluentValidation
- **Architecture**: CQRS, Generic Repository, Layered Services

---

## 🚀 Running Locally

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL instance (local or Docker)

### Steps

```bash
# 1. Clone the repository
git clone https://github.com/your-username/notes-api.git
cd notes-api

# 2. Configure your connection string in appsettings.Development.json
# "ConnectionStrings": { "Default": "Host=localhost;Database=notesdb;Username=postgres;Password=yourpassword" }

# 3. Apply database migrations
dotnet ef database update

# 4. Run the API
dotnet run --project Notes
```

The API will be available at `https://localhost:5001` with Swagger UI at `/swagger`.

### Docker (PostgreSQL only)

```bash
docker run --name notes-postgres \
  -e POSTGRES_PASSWORD=yourpassword \
  -e POSTGRES_DB=notesdb \
  -p 5432:5432 \
  -d postgres:16
```

---

## 📁 Project Structure

```
/
├── Notes/
│   ├── Controllers/          # Thin API controllers
│   ├── Service/              # Business logic layer
│   ├── DTOs/                 # Request & Response records
│   ├── Middlewares/          # Telemetry & exception handling
│   └── Program.cs
├── NotesDataAccess/
│   ├── Repositories/         # Generic & specific repositories
│   ├── Migrations/           # EF Core migrations
│   └── NotesDbContext.cs
└── README.md
```

---

## 🩺 Fixing the Deployment Issue

The production error (`relation "Notes" does not exist`) occurs because migrations were not run against the production database. To resolve:

**Option A — Apply migrations on startup** (add to `Program.cs`):
```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
    await db.Database.MigrateAsync();
}

**Option B — Create  migration service and run it when required
---



<img width="1231" height="623" alt="Untitled Diagram drawio" src="https://github.com/user-attachments/assets/6cf9c287-062d-4c33-9c61-0b3cf340d2fb" />


*Built with ❤️ for Traton Financial Services.*
