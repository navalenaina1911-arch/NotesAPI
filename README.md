This project demonstrates a .NET 10 API built for Traton Financial Services assignment. The API is designed for scalability, maintainability, and production-readiness, following modern architecture practices.
Note: Backend runs locally. In the deployed setup, PostgreSQL migrations had not executed, so the production database schema was not created. This is a deployment configuration issue and fully fixable.
API DesignVersioning: All endpoints follow api/v1Pattern: CQRS (Command/Query Responsibility Segregation)
Controllers: Thin, delegate logic to service layer
Service Layer: Handles business logic
Repository Layer: Generic repository (T pattern) for reusable CRUD
DTOs: Request and Response objects using records
Validation: FluentValidation for all request models
Exception Handling & Telemetry Middleware: Structured logging for observability
CancellationToken: Passed through all layers for graceful request cancellationEndpoints
Endpoint Details
GET /api/v1/notes/getAll Retrieve all notes N/A List
GET /api/v1/notes/{id} Retrieve note by ID
POST /api/v1/notes Create a new note CreateNoteRequestDTO
PUT /api/v1/notes/{id} Update a note UpdateNoteRequestDTO
DELETE /api/v1/notes/{id} Soft delete a note
Features:
Soft Delete: Implemented via IsDeleted column with HasQueryFilter for automatic exclusion from queries.
Full-text Search: PostgreSQL tsvector column with HasGeneratedTsVectorColumn and GIN index for fast keyword search.
