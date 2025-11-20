# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Telegram Constructor** is a WPF desktop application for creating, managing, and deploying Telegram bots and userbots through a visual interface. The application eliminates the need for programming knowledge while providing advanced customization for power users.

## Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────┐
│    Presentation (WPF + MVVM)        │  TelegramConstructor.WPF
├─────────────────────────────────────┤
│    Application (Use Cases, CQRS)    │  TelegramConstructor.Application
├─────────────────────────────────────┤
│    Domain (Entities, Value Objects) │  TelegramConstructor.Domain
├─────────────────────────────────────┤
│    Infrastructure (Data, External)  │  TelegramConstructor.Infrastructure
└─────────────────────────────────────┘
```

### Layer Responsibilities

**Domain Layer** (`Core/TelegramConstructor.Domain/`)
- Contains core business entities: `Project`, `Bot`, `Command`
- Defines value objects like `BotToken`, `ApiCredentials`
- All entities inherit from `BaseEntity` with `Id`, `CreatedAt`, `UpdatedAt`
- Rich domain models with encapsulated state (private setters)
- Domain logic enforced through entity methods (e.g., `Project.AddBot()`, `Bot.AddCommand()`)

**Application Layer** (`Core/TelegramConstructor.Application/`)
- Uses **MediatR** for CQRS pattern (commands/queries)
- Uses **FluentValidation** for input validation
- Uses **AutoMapper** for DTO mapping
- Defines interfaces for infrastructure services (`ICodeGenerator`, `ITelegramService`, etc.)

**Infrastructure Layer** (`Infrastructure/TelegramConstructor.Infrastructure/`)
- **Entity Framework Core 8.0** with **SQLite** database
- `ApplicationDbContext` configures entities and relationships
- Repository pattern implementation (`ProjectRepository`, etc.)
- Telegram integration via **Telegram.Bot** and **WTelegramClient**

**Presentation Layer** (`Presentation/TelegramConstructor.WPF/`)
- **WPF** with **.NET 8.0-windows** target framework
- **Prism** framework (v9.x) for MVVM, DI, and navigation
- **MaterialDesignThemes** (v5.x) for modern UI components
- **Serilog** for structured logging (console + rolling file)
- Dependency injection configured in `App.xaml.cs`

## Development Commands

### Build and Run

```powershell
# Build the solution (from repository root)
cd ..
dotnet build TelegramConstructor.sln

# Build specific project
dotnet build src/Presentation/TelegramConstructor.WPF/TelegramConstructor.WPF.csproj

# Run the WPF application
dotnet run --project src/Presentation/TelegramConstructor.WPF/TelegramConstructor.WPF.csproj
```

### Database Operations

```powershell
# Add new migration
dotnet ef migrations add MigrationName --project src/Infrastructure/TelegramConstructor.Infrastructure --startup-project src/Presentation/TelegramConstructor.WPF

# Update database to latest migration
dotnet ef database update --project src/Infrastructure/TelegramConstructor.Infrastructure --startup-project src/Presentation/TelegramConstructor.WPF

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/Infrastructure/TelegramConstructor.Infrastructure --startup-project src/Presentation/TelegramConstructor.WPF
```

Note: The application currently uses `Database.EnsureCreated()` on startup, which creates the database without migrations. For production, switch to proper migrations.

### Testing

```powershell
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## Key Design Patterns

**Repository Pattern**: Data access abstraction via `IProjectRepository` interface (Infrastructure/TelegramConstructor.Infrastructure/Repositories/)

**Unit of Work**: Transactions managed through DbContext SaveChanges

**CQRS**: Commands and Queries separated via MediatR (Application layer)

**MVVM**: Presentation separated from business logic using ViewModels

**Dependency Injection**: Prism's Unity container (`App.xaml.cs` - `RegisterTypes()`)

## Domain Model Relationships

```
Project (1) ──→ (N) Bot (1) ──→ (N) Command
```

- **Project**: Root aggregate containing bots and project settings
- **Bot**: Contains bot configuration, token (value object), and commands
- **Command**: Defines bot command triggers and responses (supports regex)

All relationships are configured with cascade delete in `ApplicationDbContext.OnModelCreating()`.

## Important Implementation Details

### Entity Base Class
All entities inherit from `BaseEntity` (Domain/Common/BaseEntity.cs) which provides:
- `Guid Id` (auto-generated)
- `DateTime CreatedAt` (set on creation)
- `DateTime? UpdatedAt` (set via `UpdateTimestamp()` method)

### Value Objects
- **BotToken**: Encapsulates token validation logic, implements equality
- **ApiCredentials**: Stores Telegram API credentials (ApiId, ApiHash)

Both are configured as owned entities in EF Core (no separate table).

### Dependency Injection Setup
The WPF app (`App.xaml.cs`) extends `PrismApplication`:
1. **CreateShell()**: Resolves and shows `MainWindow`
2. **RegisterTypes()**: Registers DbContext, repositories, and services
3. **OnInitialized()**: Ensures database is created on app startup
4. **OnExit()**: Flushes Serilog logs

### Database Configuration
- SQLite connection string: `"Data Source=telegramconstructor.db"`
- Database file created in application directory
- Logs written to `logs/log.txt` with daily rolling interval

## Current Development Stage

The project is in **early/foundational stage**:
- ✅ Domain entities defined
- ✅ Infrastructure setup (EF Core, repositories)
- ✅ Basic WPF shell with dependency injection
- ❌ No ViewModels or Views implemented yet (navigation commented out)
- ❌ Application layer largely empty (Class1.cs placeholders)
- ❌ No code generation logic implemented
- ❌ No Telegram API integration active

## Technology Stack

- **.NET 8.0** (SDK required)
- **Entity Framework Core 8.0** + SQLite
- **Prism 9.0.537** (MVVM framework)
- **MaterialDesignThemes 5.3.0** (UI components)
- **MediatR 13.1.0** (CQRS)
- **FluentValidation 12.1.0** (validation)
- **AutoMapper 15.1.0** (mapping)
- **Serilog 4.3.0** (logging)
- **Telegram.Bot** & **WTelegramClient** (planned for bot/userbot APIs)

## Adding New Features

When adding functionality, follow these steps:

1. **Domain First**: Define entities, value objects, or domain events in the Domain layer
2. **Application Logic**: Create commands/queries with handlers and validators in Application layer
3. **Infrastructure**: Implement repositories or external service integrations
4. **Presentation**: Create Views (XAML) and ViewModels following MVVM pattern
5. **Register Services**: Update `App.xaml.cs` RegisterTypes() for new dependencies

## Database Schema

The database uses **code-first** approach:
- `Projects` table: Project metadata and settings
- `Bots` table: Bot configurations with encrypted tokens
- `Commands` table: Bot command definitions with triggers and responses

Entity configurations are in `ApplicationDbContext.OnModelCreating()` (Infrastructure/Data/).

## Navigation Pattern

Prism region-based navigation (currently commented out in `App.xaml.cs`):
```csharp
// Register views for navigation
containerRegistry.RegisterForNavigation<ProjectsListView, ProjectsListViewModel>();

// Navigate to view
regionManager.RequestNavigate("ContentRegion", "ProjectsListView");
```

## Logging

Serilog is configured to write:
- Console output (all levels)
- File output (`logs/log.txt`, rolling daily)

Access via injected `ILogger` interface.

## Security Considerations

- Bot tokens should be stored encrypted (not yet implemented)
- Use DPAPI or Windows Credential Manager for sensitive data
- Validate all user input via FluentValidation
- Sanitize command patterns to prevent injection attacks
