# Clean Architecture (.NET 7, CQRS, MediatR)

## Description:
This template is based on https://github.com/jasontaylordev/CleanArchitecture with a few changes.

Goal of the template is to provide a simplified version of the original Clean Architecture template which includes only the backend API for enterprise application development.

Click **Use this template** above or install the **.NET template**.

Changes from the original template:
- Template includes only the backend API - thde default Angular frontend has been removed so you can use any client of your choice.
- IdentityServer has been replaced with a simple JWT token auth.
- Supporting classes such as Validators, EventHandlers have been moved in the same file with the relevant command/query. This makes it easier to navigate through specific features of the system since you don't have to open multiple files to find out what's going on. The only bits that have't moved are the domain layer entities.
- IdentityService class has been expanded and now has extra authorization methods.

## Getting Started

Get started by installing the [.NET template](https://www.nuget.org/packages/Clean.Architecture.Solution.Template) and run `dotnet new ca-api`:

1. Install the latest [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
2. Run `dotnet new install Clean.Architecture.Solution.Template` to install the .NET template
3. Create a folder for your solution and cd into it (the template will use it as project name)
4. Run `dotnet new ca-sln` to create a new project

## Database Configuration:
By default solution is configured to use an in-memory database by default. This ensures that all users will be able to run the solution without needing to set up additional infrastructure (e.g. SQL Server).

If you would like to use SQL Server, you will need to update *Api/appsettings.json* as follows: 
`"UseInMemoryDatabase": false`

Verify that the DefaultConnection connection string within appsettings.json points to a valid SQL Server instance.

When you run the application the database will be automatically created (if necessary) and the latest migrations will be applied.

## Database Migrations
To use dotnet-ef for your migrations first ensure that "UseInMemoryDatabase" is disabled, as described within previous section. Then, add the following flags to your command (values assume you are executing from repository root)

--project src/Infrastructure
--startup-project src/Api
--output-dir Persistence/Migrations

For example, to add a new migration from the root folder:

`dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Api --output-dir Persistence\Migrations`

## Overview
#### Domain
Contains all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

#### Application
Contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

#### Infrastructure
Contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

#### Api
Depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only Startup.cs should reference Infrastructure.
